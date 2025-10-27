using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject loginPanel;
    public GameObject usernameInputPanel;
    public GameObject gameGuideTotalPanel;
    public GameObject nextButton;

    private void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    /// <summary>
    /// ESC 키로 열려 있는 패널을 닫는 처리
    /// </summary>
    private void HandleEscape()
    {
        // 우선순위: GameExplanation → InputPanel → LoginPanel
        if (gameGuideTotalPanel != null && gameGuideTotalPanel.activeSelf)
        {
            ClosePanel(gameGuideTotalPanel);
            return;
        }

        if (usernameInputPanel != null && usernameInputPanel.activeSelf)
        {
            ClosePanel(usernameInputPanel);

            // ESC로 닫으면 NextButton 활성화
            if (nextButton != null)
                nextButton.SetActive(true);

            return;
        }

        if (loginPanel != null && loginPanel.activeSelf)
        {
            ClosePanel(loginPanel);
            return;
        }
    }


    /// <summary>
    /// 지정한 패널을 닫는다.
    /// </summary>
    private void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// 다른 스크립트에서 패널을 열 때 사용
    /// </summary>
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
}
