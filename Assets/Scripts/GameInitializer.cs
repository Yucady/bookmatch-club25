using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private AllBooksQuizData allBooksData;
    [SerializeField] private QuizManager quizManager;

    private void Start()
    {
        Debug.Log("GameInitializer Start called");
        string studentNumber = PlayerPrefs.GetString("StudentNumber", "");

        if (string.IsNullOrEmpty(studentNumber))
        {
            Debug.LogError("학번 데이터가 없습니다. 메인 메뉴로 돌아갑니다.");
            ReturnToMainMenu();
            return;
        }

        if (allBooksData == null || allBooksData.books == null || allBooksData.books.Length == 0)
        {
            Debug.LogError("책 데이터가 없습니다. 게임을 시작할 수 없습니다.");
            ReturnToMainMenu();
            return;
        }

        if (quizManager != null)
        {
            float initialTime = 180f; // 3분(180초) 초기 시간, 필요에 따라 조정
            quizManager.StartGame(studentNumber, initialTime);
        }
        else
        {
            Debug.LogError("QuizManager가 할당되지 않았습니다.");
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
