using UnityEngine;

public class SetGameResolution : MonoBehaviour
{
    [Header("고정 게임 해상도")]
    public int targetWidth = 1500;
    public int targetHeight = 1000;

    void Awake()
    {
        // 창모드 + 해상도 고정
        Screen.fullScreen = false;
        Screen.SetResolution(targetWidth, targetHeight, false);
    }
}
