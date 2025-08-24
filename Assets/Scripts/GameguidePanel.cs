using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameguidePanel : MonoBehaviour
{
    [System.Serializable]
    public struct DescriptionData
    {
        public Sprite image;
        [TextArea] public string ExplainText;
    }

    public Image displayImage;
    public TextMeshProUGUI displayText;
    public Button rightButton;
    public Button leftButton;
    public Button closeButton;
    public Button backgroundButton;
    public GameObject gameguidePanel;

    public DescriptionData[] pages;

    private int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightButton.onClick.AddListener(OnClickRight);
        leftButton.onClick.AddListener(OnClickLeft);
        closeButton.onClick.AddListener(ClosePanel);
        backgroundButton.onClick.AddListener(ClosePanel);

        UpdatePage();
    }

    void OnClickRight()
    {
        if (currentIndex < pages.Length - 1)
        {
            currentIndex++;
            UpdatePage();
        }
    }

    void OnClickLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePage();
        }
    }

    void UpdatePage()
    {
        displayImage.sprite = pages[currentIndex].image;
        displayText.text = pages[currentIndex].ExplainText;

        rightButton.interactable = currentIndex < pages.Length - 1;
        leftButton.interactable = currentIndex > 0;
    }

    void ClosePanel()
    {
        gameguidePanel.SetActive(false);
    }
}
