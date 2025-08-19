using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private AllBooksQuizData allBooksData;

    private BookQuizData currentBook;
    private QuizData currentQuiz;
    private int currentStage = 1;

    private string playerStudentNumber;
    private float remainingTime;
    private bool isTimerRunning = false;

    private bool[] judges;

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
                                             // 전부 true 일 경우 Firebase 연결 그리고 전부 true 가 아니더라도 모든 스테이지 진행
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

        
        
        //if (isCorrect) // 수정할 부분
        //{
        //    currentStage++;
        //    Debug.Log($"Moving to stage {currentStage}");
        //    if (currentStage <= 3)
        //        LoadQuiz(currentStage);
        //    else
        //        GameEnd(true);
        //}
        //else
        //{
        //    GameEnd(false);
        //}
    }


    private void GameEnd(bool isSuccess)
    {
        isTimerRunning = false;

        if (isSuccess)
        {
            FirebaseManager.Instance?.SavePlayerResult(playerStudentNumber, remainingTime);
            UIManager.Instance?.ShowSuccessPanel();
        }
        else
        {
            UIManager.Instance?.ShowFailPanel();
        }
    }
}
