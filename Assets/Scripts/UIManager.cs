using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("공통 UI")]
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI timerText;
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
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("정답 표시용 이미지")]
    [SerializeField] private Image[] judgeImages;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xSprite;

    [Header("ESC/NextButton 관련")]
    [SerializeField] private GameObject usernameInputPanel;
    [SerializeField] private GameObject gameGuideTotalPanel;
    [SerializeField] private GameObject nextButton;

    public RankUIGameScene rankingPopup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // ESC 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }

        // 주관식 Enter 제출 처리
        if (subjectivePanel.activeSelf && answerInputField.isFocused)
        {
            // IME 조합 중이면 제출 막기
            if (Input.GetKeyDown(KeyCode.Return) && string.IsNullOrEmpty(Input.compositionString))
            {
                SubmitSubjectiveAnswer();
            }
        }
    }

    #region 퀴즈 표시

    public void ShowQuiz(BookQuizData bookQuiz, QuizData quiz, int stage, QuizType quizType)
    {
        bookCoverImage.sprite = bookQuiz.coverImage;
        bookTitle.text = bookQuiz.bookTitle;
        quizCreator.text = $"제작자: {bookQuiz.quizCreator}";

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

        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(() => SubmitSubjectiveAnswer());

        StartCoroutine(EnableInputFieldFocus());

        hintText.gameObject.SetActive(false);
        hintText.text = quiz.hint;

        CancelInvoke(nameof(ShowHint));
        Invoke(nameof(ShowHint), hintDelay);
    }

    private IEnumerator EnableInputFieldFocus()
    {
        yield return null;
        answerInputField.Select();
        answerInputField.ActivateInputField();
    }

    private void SubmitSubjectiveAnswer()
    {
        string rawInput = answerInputField.text;
        string normalizedInput = rawInput.Trim()
                                         .Normalize(System.Text.NormalizationForm.FormC)
                                         .Replace("\u200B", "");

        Debug.Log($"[주관식 입력 디버그] Raw: '{rawInput}' → Normalized: '{normalizedInput}'");
        QuizManagerRef.CheckAnswer(normalizedInput);
        answerInputField.text = "";
    }

    private void ShowHint()
    {
        hintText.gameObject.SetActive(true);
    }

    #endregion

    #region 결과/랭킹

    public void ShowResultPanel(bool isSuccess, float remainingTime, bool[] judges)
    {
        resultPanel.SetActive(true);

        Image panelImage = resultPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.sprite = isSuccess ? successSprite : failSprite;

        timeText.text = $"{remainingTime:F1}";

        for (int i = 0; i < judgeImages.Length; i++)
        {
            if (i < judges.Length)
                judgeImages[i].sprite = judges[i] ? oSprite : xSprite;
            else
                judgeImages[i].sprite = null;
        }
    }

    public void ShowRankingPanel() => rankingPopup.Open();
    public void CloseRankingPanel() => rankingPanel.SetActive(false);

    public void UpdateTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    #endregion

    #region ESC 처리

    private void HandleEscape()
    {
        if (gameGuideTotalPanel != null && gameGuideTotalPanel.activeSelf)
        {
            gameGuideTotalPanel.SetActive(false);
            return;
        }

        if (usernameInputPanel != null && usernameInputPanel.activeSelf)
        {
            usernameInputPanel.SetActive(false);

            if (nextButton != null)
                nextButton.SetActive(true);

            return;
        }
    }

    #endregion

    #region 재시작

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

    #endregion

    private QuizManager QuizManagerRef => FindFirstObjectByType<QuizManager>();
}
