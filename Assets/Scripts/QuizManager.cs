using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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


    private bool[] judges;

    void Awake()
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
            GameEnd(false); // 시간 초과로 실패 처리
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
        Debug.Log($"LoadQuiz called with stage: {stage}");
        if (currentBook == null)
        {
            Debug.LogError("현재 책 데이터가 없습니다.");
            return;
        }

        if (stage == 1)
            currentQuiz = GetRandomQuiz(currentBook.stage1);
        else if (stage == 2)
            currentQuiz = GetRandomQuiz(currentBook.stage2);
        else if (stage == 3)
            currentQuiz = GetRandomQuiz(currentBook.stage3);
        else
        {
            Debug.LogWarning("알 수 없는 스테이지 번호입니다: " + stage);
            return;
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowQuiz(currentBook, currentQuiz, stage, currentQuiz.quizType);
        else
            Debug.LogError("UIManager 인스턴스를 찾을 수 없습니다.");
    }

    private QuizData GetRandomQuiz(QuizData[] quizArray)
    {
        Debug.Log("현재 스테이지(GetRandomQuiz): " + currentStage);
        if (quizArray == null || quizArray.Length == 0)
        {
            Debug.LogError("퀴즈 배열이 비어있습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, quizArray.Length); // 0 ~ 1 두 개
        return quizArray[randomIndex];
    }

    public void CheckAnswer(string playerAnswer)
    {
        Debug.Log($"CheckAnswer called with: {playerAnswer}");
        if (currentQuiz == null)
        {
            Debug.LogError("현재 퀴즈가 없습니다.");
            return;
        }

        bool isCorrect = playerAnswer.Trim().ToLower() == currentQuiz.answer.Trim().ToLower();
        Debug.Log($"Answer is correct? {isCorrect}");

        if (isCorrect)
        {
            judges[currentStage - 1] = true; // judges 에 정답 여부 저장 -> 정답 갯수에 따라 결과 패널에서의 별의 개수 변화
            //Debug.Log("judges: " + currentStage + $"{isCorrect}");
        }
        else
        {
            judges[currentStage - 1] = false;
            //Debug.Log("judges: " + currentStage + $"{isCorrect}");
        }

        currentStage++;
        Debug.Log($"Moving to stage {currentStage}");
        
        if (currentStage <= 3)
        {
            LoadQuiz(currentStage);
        }
        else
        {
            //Debug.Log("GameEnd judge is conducted");
            if (judges.All(j => j))
            {
                //Debug.Log($"judges in if(true): {judges[0]}, {judges[1]}, {judges[2]}");
                GameEnd(true);
            }
            else
            {
                //Debug.Log($"judges in if(false): {judges[0]}, {judges[1]}, {judges[2]}");
                GameEnd(false);
            }
        }
    }


    private void GameEnd(bool isSuccess)
    {
        Debug.Log($"BackendLogin.Instance: {(BackendLogin.Instance == null ? "NULL" : "OK")}");
        Debug.Log($"GameInitializer.Instance: {(GameInitializer.Instance == null ? "NULL" : "OK")}");

        isTimerRunning = false;

        if (!isSuccess)
        {
            UIManager.Instance?.ShowFailPanel();
            return;
        }

        UIManager.Instance?.ShowSuccessPanel();

        string studentNum = GameInitializer.Instance.studentNumber;

        // 로그인 상태 확인 후 처리
        if (!BackendLogin.Instance.IsLoggedIn)
        {
            BackendLogin.Instance.CustomLogin(studentNum, studentNum, loginCallback =>
            {
                if (!loginCallback.IsSuccess())
                {
                    Debug.LogError($"로그인 실패: {loginCallback}");
                    return;
                }

                Debug.Log("로그인 성공");

                // 닉네임 업데이트는 중복 시 무시
                BackendLogin.Instance.UpdateNickname(studentNum, nicknameCallback =>
                {
                    if (!nicknameCallback.IsSuccess())
                        Debug.LogWarning($"닉네임 업데이트 실패: {nicknameCallback}");
                    else
                        Debug.Log("닉네임 업데이트 성공");

                    // 모든 준비 완료 → 게임 데이터 & 랭킹 처리
                    HandleGameDataAndRank(studentNum);
                });
            });
        }
        else
        {
            // 이미 로그인 상태이면 바로 게임 데이터 & 랭킹 처리
            HandleGameDataAndRank(studentNum);
        }
    }

    // -----------------------------
    // 게임 데이터 조회/삽입/업데이트 후 최고 점수 기준 랭킹 등록
    private void HandleGameDataAndRank(string studentNum)
    {
        BackendGameData.Instance.GameDataGet(getCallback =>
        {
            if (BackendGameData.userData == null)
            {
                // 데이터 없으면 삽입
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
                // 데이터 존재하면 업데이트
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
    }
}
