using System.Linq;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    [SerializeField] private AllBooksQuizData allBooksData;

    private BookQuizData currentBook;
    private QuizData currentQuiz;
    private int currentStage = 1;

    private string playerStudentNumber;
    private bool isTimerRunning = false;
    public float remainingTime { get; private set; }

    private bool[] judges; // 각 스테이지의 정답 여부
    private bool gameEnded = false; // 중복 GameEnd 방지

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame(string studentNumber, float startTime)
    {
        Debug.Log($"StartGame called with studentNumber: {studentNumber}");

        if (allBooksData == null || allBooksData.books == null || allBooksData.books.Length == 0)
        {
            Debug.LogError("퀴즈 데이터가 없습니다.");
            return;
        }

        playerStudentNumber = studentNumber;
        remainingTime = startTime;
        judges = new bool[3];
        currentBook = GetRandomBook();
        currentStage = 1;
        gameEnded = false;

        LoadQuiz(currentStage);
        isTimerRunning = true;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isTimerRunning = false;
            GameEnd(false); // 시간 초과
        }

        UIManager.Instance?.UpdateTimer(remainingTime);
    }

    private BookQuizData GetRandomBook()
    {
        int randomIndex = Random.Range(0, allBooksData.books.Length);
        return allBooksData.books[randomIndex];
    }

    private void LoadQuiz(int stage)
    {
        if (currentBook == null)
        {
            Debug.LogError("현재 책 데이터가 없습니다.");
            return;
        }

        switch (stage)
        {
            case 1:
                currentQuiz = GetRandomQuiz(currentBook.stage1);
                break;
            case 2:
                currentQuiz = GetRandomQuiz(currentBook.stage2);
                break;
            case 3:
                currentQuiz = GetRandomQuiz(currentBook.stage3);
                break;
            default:
                Debug.LogWarning($"잘못된 스테이지 번호: {stage}");
                return;
        }

        if (currentQuiz == null)
        {
            Debug.LogError($"스테이지 {stage} 퀴즈를 불러오지 못했습니다.");
            return;
        }

        UIManager.Instance?.ShowQuiz(currentBook, currentQuiz, stage, currentQuiz.quizType);
    }

    private QuizData GetRandomQuiz(QuizData[] quizArray)
    {
        if (quizArray == null || quizArray.Length == 0)
        {
            Debug.LogError("퀴즈 배열이 비어있습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, quizArray.Length);
        return quizArray[randomIndex];
    }

    public void CheckAnswer(string playerAnswer)
    {
        if (gameEnded) return; // 이미 끝난 게임 방지
        if (currentQuiz == null)
        {
            Debug.LogError("현재 퀴즈가 없습니다.");
            return;
        }

        bool isCorrect = playerAnswer.Trim().ToLower() == currentQuiz.answer.Trim().ToLower();
        judges[currentStage - 1] = isCorrect;

        currentStage++;
        if (currentStage <= 3)
        {
            LoadQuiz(currentStage);
        }
        else
        {
            bool allCorrect = judges.All(j => j);
            GameEnd(allCorrect);
        }
    }

    private void GameEnd(bool isSuccess)
    {
        if (gameEnded) return;
        gameEnded = true;
        isTimerRunning = false;

        // 결과 패널 표시 + 남은 시간 표시
        UIManager.Instance?.ShowResultPanel(isSuccess, remainingTime, judges);

        Debug.Log($"결과 판정: {judges[0]}, {judges[1]}, {judges[2]}");

        string studentNum = GameInitializer.Instance?.studentNumber ?? playerStudentNumber;
        if (string.IsNullOrEmpty(studentNum))
        {
            Debug.LogError("학생 번호 정보가 없습니다. 랭킹 등록 불가.");
            return;
        }

        // 비로그인 상태 → 로그인 후 랭킹 처리
        if (!BackendLogin.Instance.IsLoggedIn)
        {
            BackendLogin.Instance.CustomLogin(studentNum, studentNum, loginCallback =>
            {
                if (!loginCallback.IsSuccess())
                {
                    Debug.LogError($"로그인 실패: {loginCallback}");
                    return;
                }

                BackendLogin.Instance.UpdateNickname(studentNum, nicknameCallback =>
                {
                    if (!nicknameCallback.IsSuccess())
                        Debug.LogWarning("닉네임 업데이트 실패");
                    else
                        Debug.Log("닉네임 업데이트 성공");

                    HandleGameDataAndRank(studentNum);
                });
            });
        }
        else
        {
            HandleGameDataAndRank(studentNum);
        }
    }

    private void HandleGameDataAndRank(string studentNum)
    {
        BackendGameData.Instance.GameDataGet(getCallback =>
        {
            if (BackendGameData.userData == null)
            {
                BackendGameData.Instance.GameDataInsert(remainingTime, studentNum, insertCallback =>
                {
                    if (insertCallback.IsSuccess())
                    {
                        Debug.Log("게임 데이터 삽입 완료");
                        BackendRank.Instance.RankInsertHighScore(remainingTime, studentNum);
                        BackendRank.Instance.RankGet();
                    }
                    else
                    {
                        Debug.LogError("게임 데이터 삽입 실패: " + insertCallback);
                    }
                });
            }
            else
            {
                BackendGameData.Instance.GameDataUpdate(updateCallback =>
                {
                    if (updateCallback.IsSuccess())
                    {
                        Debug.Log("게임 데이터 업데이트 완료");
                        BackendRank.Instance.RankInsertHighScore(remainingTime, studentNum);
                        BackendRank.Instance.RankGet();
                    }
                    else
                    {
                        Debug.LogError("게임 데이터 업데이트 실패: " + updateCallback);
                    }
                });
            }
        });
    }

    public void ResetData()
    {
        currentStage = 1;
        remainingTime = 0f;
        isTimerRunning = false;
        judges = null;
        currentBook = null;
        currentQuiz = null;
        playerStudentNumber = null;
        gameEnded = false;
    }
}
