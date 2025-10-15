using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("공통 UI")]
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI timerText;  // 타이머 텍스트
    [SerializeField] private Image bookCoverImage;
    [SerializeField] private TextMeshProUGUI bookTitle;  
    [SerializeField] private TextMeshProUGUI quizCreator; 

    [Header("퀴즈 패널")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI questionText;

    [Header("1단계 OX")]
    [SerializeField] private GameObject oxPanel;
    [SerializeField] private Button oButton;
    [SerializeField] private Button xButton;

    [Header("2단계 객관식")]
    [SerializeField] private GameObject multipleChoicePanel;
    [SerializeField] private Button[] choiceButtons;

    [Header("3단계 주관식")]
    [SerializeField] private GameObject subjectivePanel;
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDelay = 5f;

    [Header("결과 패널")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private TextMeshProUGUI resultText;

    public RankUIGameScene rankingPopup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowQuiz(BookQuizData bookQuiz, QuizData quiz, int stage, QuizType quizType)
    {
        Debug.Log($"ShowQuiz called: stage={stage}, quizType={quizType}, question={quiz.question}");

        bookCoverImage.sprite = bookQuiz.coverImage;
        bookTitle.text = bookQuiz.bookTitle;           // 책 제목 표시
        quizCreator.text = $"제작자: {bookQuiz.quizCreator}"; // 퀴즈 제작자 표시

        stageText.text = $"{stage}단계";
        questionText.text = quiz.question;

        // 모든 유형 패널 비활성화
        oxPanel.SetActive(false);
        multipleChoicePanel.SetActive(false);
        subjectivePanel.SetActive(false);

        switch (quizType)
        {
            case QuizType.OX:
                ShowOXQuiz(quiz);
                break;
            case QuizType.Multiple:
                ShowMultipleChoiceQuiz(quiz);
                break;
            case QuizType.Subjective:
                ShowSubjectiveQuiz(quiz);
                break;
        }
    }

    private void ShowOXQuiz(QuizData quiz)
    {
        oxPanel.SetActive(true);

        oButton.onClick.RemoveAllListeners();
        xButton.onClick.RemoveAllListeners();

        oButton.onClick.AddListener(() => QuizManagerRef.CheckAnswer("O"));
        xButton.onClick.AddListener(() => QuizManagerRef.CheckAnswer("X"));
    }

    private void ShowMultipleChoiceQuiz(QuizData quiz)
    {
        multipleChoicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < quiz.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = quiz.choices[i];
                int idx = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => QuizManagerRef.CheckAnswer(quiz.choices[idx]));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowSubjectiveQuiz(QuizData quiz)
    {
        subjectivePanel.SetActive(true);
        answerInputField.text = "";

        // Submit 버튼 클릭 이벤트
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => SubmitSubjectiveAnswer());

        // TMP InputField Enter 키 이벤트
        answerInputField.onSubmit.RemoveAllListeners();
        answerInputField.onSubmit.AddListener(delegate { SubmitSubjectiveAnswer(); });

        // InputField 활성화 → 아무 키 입력 가능
        StartCoroutine(EnableInputFieldFocus());

        // 힌트 초기화
        hintText.gameObject.SetActive(false);
        hintText.text = quiz.hint;

        CancelInvoke(nameof(ShowHint));
        Invoke(nameof(ShowHint), hintDelay);
    }

    private System.Collections.IEnumerator EnableInputFieldFocus()
    {
        yield return null; // 다음 프레임까지 대기 (TMP InputField 초기화 보장)
        answerInputField.Select();
        answerInputField.ActivateInputField();
    }

    private void SubmitSubjectiveAnswer()
    {
        QuizManagerRef.CheckAnswer(answerInputField.text);
        answerInputField.text = ""; // 제출 후 초기화
    }

    void Update()
    {
        // Enter 키 입력으로 제출
        if (subjectivePanel.activeSelf && answerInputField.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            SubmitSubjectiveAnswer();
        }
    }

    private void ShowHint()
    {
        hintText.gameObject.SetActive(true);
    }

    public void ShowFailPanel()
    {
        resultPanel.SetActive(true);
        resultText.text = "실패!";

        Image panelImage = resultPanel.GetComponent<Image>();
        if (panelImage != null && failSprite != null)
            panelImage.sprite = failSprite;
    }

    public void ShowSuccessPanel()
    {
        resultPanel.SetActive(true);
        resultText.text = "성공!";

        Image panelImage = resultPanel.GetComponent<Image>();
        if (panelImage != null && successSprite != null)
            panelImage.sprite = successSprite;
    }


    public void ShowRankingPanel()
    {
        rankingPopup.Open();
    }

    public void CloseRankingPanel()
    {
        rankingPanel.SetActive(false);
    }

    public void UpdateTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // ---------------------------------------------
    // 뒤끝 로그아웃 + 게임 재시작
    // ---------------------------------------------
    public void OnRestartButtonClicked()
    {
        Debug.Log("Restart 버튼 클릭됨. 게임 상태 완전 초기화 진행.");

        var logout = Backend.BMember.Logout();
        if (logout.IsSuccess())
            Debug.Log("뒤끝 로그아웃 성공");
        else
            Debug.LogWarning("뒤끝 로그아웃 실패: " + logout);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        var quizMgr = FindFirstObjectByType<QuizManager>();
        if (quizMgr != null)
            quizMgr.ResetData();

        if (GameInitializer.Instance != null)
            Destroy(GameInitializer.Instance.gameObject);

        var backendMgr = FindFirstObjectByType<BackendManager>();
        if (backendMgr != null)
            Destroy(backendMgr.gameObject);

        SceneManager.LoadScene("MainMenuScene");
    }

    private QuizManager QuizManagerRef => FindFirstObjectByType<QuizManager>();
}
