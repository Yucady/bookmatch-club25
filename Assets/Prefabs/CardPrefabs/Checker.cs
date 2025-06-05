using UnityEngine;
using UnityEngine.UI;

public class Checker : MonoBehaviour
{
    public Button cardButton;

    void Start()
    {
        Debug.Log("Hello world!");
        cardButton.onClick.AddListener(OnCardClicked);
    }

    void OnCardClicked()
    {
        Debug.Log("Ä«µå Å¬¸¯µÊ");
    }

}
