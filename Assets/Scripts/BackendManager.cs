using UnityEngine;
using BackEnd;
using System;

public class BackendManager : MonoBehaviour
{
    public static event Action OnBackendInitialized;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var bro = Backend.Initialize();

        if (bro.IsSuccess())
        {
            Debug.Log("Backend 초기화 성공: " + bro);
            OnBackendInitialized?.Invoke(); // 초기화 완료 알림
        }
        else
        {
            Debug.LogError("Backend 초기화 실패: " + bro);
        }
    }
}
