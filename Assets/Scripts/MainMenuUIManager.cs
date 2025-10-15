using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject loginPanel;
    public GameObject usernameInputPanel;
    public GameObject gameGuideTotalPanel;

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
        if (usernameInputPanel != null && usernameInputPanel.activeSelf)
        {
            ClosePanel(usernameInputPanel);
            return;
        }

        if (usernameInputPanel != null && usernameInputPanel.activeSelf)
        {
            ClosePanel(usernameInputPanel);
            return;
        }

        if (loginPanel != null && loginPanel.activeSelf)
        {
            ClosePanel(loginPanel);
            return;
        }

        // 모두 닫혀 있으면 무시 (또는 종료 처리 가능)
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
