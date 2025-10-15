using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject playerInputPanel;
    [SerializeField] private TMP_InputField studentNumberInput;
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("애니메이션")]
    [SerializeField] private Image animationImage; // 결과 애니메이션 표시할 Image
    [SerializeField] private Sprite[] animationFrames; // PNG 25장
    [SerializeField] private float frameRate = 0.04f; // 25FPS = 1/25초

    [Header("씬 이동 설정")]
    [SerializeField] private string nextSceneName = "GameScene";

    private void Start()
    {
        // Enter 키로 입력 제출 가능
        studentNumberInput.onSubmit.AddListener(delegate { OnConfirmButton(); });
    }

    private void Update()
    {
        if (studentNumberInput.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            OnConfirmButton();
        }
    }

    public void OnPlayButton()
    {
        playerInputPanel.SetActive(true);
        warningText.text = "";
        studentNumberInput.text = "";
        studentNumberInput.Select(); // 자동 포커스
    }

    public void OnConfirmButton()
    {
        string input = studentNumberInput.text.Trim();

        if (IsValidStudentNumber(input))
        {
            PlayerPrefs.SetString("StudentNumber", input);
            // 입력 즉시 애니메이션 재생 후 씬 이동
            StartCoroutine(PlayAnimationAndLoadScene());
        }
        else
        {
            warningText.text = "학번은 4자리 숫자여야 합니다.";
            studentNumberInput.text = "";
            studentNumberInput.Select();
        }
    }

    private bool IsValidStudentNumber(string input)
    {
        if (input.Length != 4) return false;
        foreach (char c in input)
        {
            if (!char.IsDigit(c)) return false;
        }
        return true;
    }

    private IEnumerator PlayAnimationAndLoadScene()
    {
        // 애니메이션 이미지 활성화
        animationImage.gameObject.SetActive(true);

        // 프레임 순서대로 재생
        foreach (var frame in animationFrames)
        {
            animationImage.sprite = frame;
            yield return new WaitForSeconds(frameRate);
        }

        // 애니메이션 종료 후 씬 이동
        SceneManager.LoadScene(nextSceneName);
    }
}
