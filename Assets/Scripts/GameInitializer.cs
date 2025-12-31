using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }

    [SerializeField] private AllBooksQuizData allBooksData;
    [SerializeField] private QuizManager quizManager;

    public string studentNumber { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        studentNumber = PlayerPrefs.GetString("StudentNumber", null);
        Debug.Log($"[GameInitializer] Initialized with studentNumber = {studentNumber}");
    }

    private void Start()
    {
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

        float initialTime = 180f;

        // 회원가입 → 로그인 → 닉네임 업데이트 → 게임 시작
        BackendLogin.Instance.CustomSignUp(studentNumber, studentNumber, authBro =>
        {
            if (!authBro.IsSuccess())
            {
                Debug.LogError("회원가입/로그인 실패. 메인 메뉴로 돌아갑니다.");
                ReturnToMainMenu();
                return;
            }

            // 닉네임 한 번만 갱신
            BackendLogin.Instance.UpdateNickname(studentNumber, nickBro =>
            {
                if (!nickBro.IsSuccess())
                    Debug.LogWarning("닉네임 업데이트 실패, 계속 진행합니다.");

                // 게임 시작
                quizManager.StartGame(studentNumber, initialTime);
            });
        });
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
