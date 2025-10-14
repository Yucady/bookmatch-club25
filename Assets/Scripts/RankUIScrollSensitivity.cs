using UnityEngine;
using UnityEngine.UI;

public class RankUIScrollSensitivity : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float sensitivity = 30f;

    void Start()
    {
        scrollRect.scrollSensitivity = sensitivity;
    }
}
