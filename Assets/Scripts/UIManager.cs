using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("공통 UI")]
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI timerText;  // 타이머 텍스트 추가
    [SerializeField] private Image bookCoverImage;

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
    [SerializeField] private TextMeshProUGUI resultText;

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
        submitButton.onClick.AddListener(() => QuizManagerRef.CheckAnswer(answerInputField.text));

        // 힌트 초기화
        hintText.gameObject.SetActive(false);
        hintText.text = quiz.hint;

        CancelInvoke(nameof(ShowHint));
        Invoke(nameof(ShowHint), hintDelay);
    }

    private void ShowHint()
    {
        hintText.gameObject.SetActive(true);
    }

    public void ShowFailPanel()
    {
        resultPanel.SetActive(true);
        resultText.text = "실패!";
    }

    public void ShowSuccessPanel()
    {
        resultPanel.SetActive(true);
        resultText.text = "성공!";
    }

    /// <summary>
    /// 타이머 텍스트 업데이트 함수 추가
    /// </summary>
    public void UpdateTimer(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private QuizManager QuizManagerRef => FindFirstObjectByType<QuizManager>();
}
