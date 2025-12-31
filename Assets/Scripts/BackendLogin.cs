using System;
using UnityEngine;
using BackEnd;

public class BackendLogin
{
    private static BackendLogin _instance;
    public static BackendLogin Instance => _instance ??= new BackendLogin();

    public bool IsLoggedIn { get; private set; }

    // -----------------------------
    // 회원가입 (비동기 콜백 지원)
    public void CustomSignUp(string id, string pw, Action<BackendReturnObject> callback)
    {
        var bro = Backend.BMember.CustomSignUp(id, pw);
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 성공: " + bro);
            IsLoggedIn = true;
            callback?.Invoke(bro);
        }
        else
        {
            Debug.LogWarning("회원가입 실패: " + bro);
            Debug.Log("기존 계정으로 로그인 시도");
            CustomLogin(id, pw, callback);
        }
    }

    // -----------------------------
    // 로그인 (비동기 콜백 지원)
    public void CustomLogin(string id, string pw, Action<BackendReturnObject> callback)
    {
        var bro = Backend.BMember.CustomLogin(id, pw);
        if (bro.IsSuccess())
        {
            Debug.Log("로그인 성공: " + id);
            IsLoggedIn = true;
            callback?.Invoke(bro);
        }
        else
        {
            Debug.LogError("로그인 실패: " + bro);
            IsLoggedIn = false;
            callback?.Invoke(bro);
        }
    }

    // -----------------------------
    // 닉네임 업데이트 (비동기 콜백 지원)
    public void UpdateNickname(string nickname, Action<BackendReturnObject> callback)
    {
        var bro = Backend.BMember.UpdateNickname(nickname);
        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경 성공: " + bro);
        }
        else
        {
            Debug.LogWarning("닉네임 변경 실패: " + bro);
        }

        callback?.Invoke(bro);
    }
}
