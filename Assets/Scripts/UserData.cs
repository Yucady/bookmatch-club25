using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BackEnd;

public class UserData
{
    public float remainingTime = 0f;

    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"remainingTime : {remainingTime}");
        return result.ToString();
    }
}

public class BackendGameData
{
    private static BackendGameData _instance = null;
    public static BackendGameData Instance => _instance ??= new BackendGameData();

    public static UserData userData;
    private string gameDataRowInDate = string.Empty;

    // -----------------------------
    // 게임 데이터 삽입 (비동기 콜백 지원)
    public void GameDataInsert(float remainingTime, string nickname, System.Action<BackendReturnObject> callback)
    {
        if (userData == null) userData = new UserData();

        userData.remainingTime = remainingTime;

        Param param = new Param();
        param.Add("remainingTime", userData.remainingTime);
        param.Add("nickname", nickname);

        var bro = Backend.GameData.Insert("USER_DATA_for_RANK", param);

        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 데이터 삽입 성공: " + bro);
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("게임 정보 데이터 삽입 실패: " + bro);
        }

        callback?.Invoke(bro); // 콜백 호출
    }



    // -----------------------------
    // 게임 데이터 조회 (비동기 콜백 지원)
    public void GameDataGet(System.Action<BackendReturnObject> callback = null)
    {
        var bro = Backend.GameData.GetMyData("USER_DATA_for_RANK", new Where());
        if (bro.IsSuccess())
        {
            LitJson.JsonData gameDataJson = bro.FlattenRows();

            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
                userData = null;
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"]?.ToString() ?? "";
                userData = new UserData();

                string remainingTimeStr = gameDataJson[0]["remainingTime"]?.ToString();
                if (!float.TryParse(remainingTimeStr, out userData.remainingTime))
                {
                    Debug.LogWarning($"remainingTime 파싱 실패: '{remainingTimeStr}'. 기본값 0으로 설정합니다.");
                    userData.remainingTime = 0f;
                }

                Debug.Log("게임 데이터 조회 성공: " + userData.ToString());
            }
        }
        else
        {
            Debug.LogError("게임 데이터 조회 실패: " + bro);
            userData = null;
        }

        callback?.Invoke(bro);
    }

    // -----------------------------
    // 게임 데이터 업데이트 (비동기 콜백 지원)
    public void GameDataUpdate(System.Action<BackendReturnObject> callback = null)
    {
        if (userData == null)
        {
            Debug.LogError("업데이트할 게임 데이터가 존재하지 않습니다. Insert 또는 Get 후 실행해주세요.");
            callback?.Invoke(null);
            return;
        }

        Param param = new Param();
        param.Add("remainingTime", userData.remainingTime);

        BackendReturnObject bro = null;
        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            bro = Backend.GameData.Update("USER_DATA_for_RANK", new Where(), param);
            Debug.Log("최신 게임 데이터 업데이트 요청");
        }
        else
        {
            bro = Backend.GameData.UpdateV2("USER_DATA_for_RANK", gameDataRowInDate, Backend.UserInDate, param);
            Debug.Log($"{gameDataRowInDate} 데이터 업데이트 요청");
        }

        if (bro.IsSuccess())
        {
            Debug.Log("게임 데이터 업데이트 성공: " + bro);
        }
        else
        {
            Debug.LogError("게임 데이터 업데이트 실패: " + bro);
        }

        callback?.Invoke(bro);
    }
}
