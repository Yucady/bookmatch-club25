using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject playerInputPanel;
    [SerializeField] private TMP_InputField studentNumberInput;
    [SerializeField] private TextMeshProUGUI warningText;

    public void OnPlayButton()
    {
        playerInputPanel.SetActive(true);
        warningText.text = "";
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
