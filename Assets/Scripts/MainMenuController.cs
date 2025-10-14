using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject playerInputPanel;
    [SerializeField] private TMP_InputField studentNumberInput;
    [SerializeField] private TextMeshProUGUI warningText;

    void Start()
    {
        // Enter 키로 입력 제출 가능하게 설정
        studentNumberInput.onSubmit.AddListener(delegate { OnConfirmButton(); });
    }

    void Update()
    {
        // 입력창에 포커스된 상태에서 Enter 누르면 확인 처리
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
        studentNumberInput.Select();  // 자동 포커스
    }

    public void OnConfirmButton()
    {
        string input = studentNumberInput.text.Trim();

        if (IsValidStudentNumber(input))
        {
            PlayerPrefs.SetString("StudentNumber", input);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            warningText.text = "학번은 4자리 숫자여야 합니다.";
            studentNumberInput.text = "";
            studentNumberInput.Select(); // 다시 입력창 포커스
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
}
