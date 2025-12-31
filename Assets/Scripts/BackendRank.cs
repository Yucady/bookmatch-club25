using BackEnd;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BackendRank
{
    private static BackendRank _instance = null;
    public static BackendRank Instance => _instance ??= new BackendRank();

    // -----------------------------
    // 최고 점수 기준 랭킹 등록 (비동기 콜백 지원)
    public void RankInsertHighScore(float remainingTime, string nickname)
    {
        string rankUUID = "01993980-12fe-7756-b453-36dcb211d312"; // 랭킹 UUID
        string tableName = "USER_DATA_for_RANK";

        // 게임 데이터 rowInDate 가져오기
        var bro = Backend.GameData.GetMyData(tableName, new Where());
        string rowInDate = "";

        if (bro.IsSuccess() && bro.FlattenRows().Count > 0)
        {
            rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
        }
        else
        {
            Debug.LogWarning("게임 데이터 rowInDate를 가져오지 못했습니다. 랭킹 등록을 시도합니다.");
        }

        Param param = new Param();
        param.Add("remainingTime", remainingTime);

        // 기존 점수 확인
        float existingScore = 0f;
        if (bro.IsSuccess() && bro.FlattenRows().Count > 0)
        {
            string scoreStr = bro.FlattenRows()[0]["remainingTime"]?.ToString();
            float.TryParse(scoreStr, out existingScore);
        }

        // 동일 점수라도 강제 등록
        if (remainingTime >= existingScore)
        {
            var rankBro = Backend.URank.User.UpdateUserScore(rankUUID, tableName, rowInDate, param);
            if (rankBro.IsSuccess())
            {
                Debug.Log("랭킹 등록 완료 (동일 점수도 포함): " + rankBro);
            }
            else
            {
                Debug.LogError("랭킹 등록 실패: " + rankBro);
            }
        }
        else
        {
            Debug.Log($"랭킹 갱신하지 않음. 기존 점수({existingScore})가 새 점수({remainingTime})보다 높음.");
        }
    }



    // -----------------------------
    // 랭킹 조회 (비동기 콜백 지원)
    public void RankGet(System.Action<BackendReturnObject> callback = null)
    {
        string rankUUID = "01993980-12fe-7756-b453-36dcb211d312";
        var bro = Backend.URank.User.GetRankList(rankUUID);

        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 조회 실패: " + bro);
            callback?.Invoke(bro);
            return;
        }

        Debug.Log("랭킹 조회 성공: " + bro);
        Debug.Log("총 랭킹 등록 유저 수 : " + bro.GetFlattenJSON()["totalCount"].ToString());

        foreach (LitJson.JsonData jsonData in bro.FlattenRows())
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine("순위 : " + jsonData["rank"].ToString());
            info.AppendLine("닉네임 : " + jsonData["nickname"].ToString());
            info.AppendLine("점수 : " + jsonData["score"].ToString());
            info.AppendLine("gamerInDate : " + jsonData["gamerInDate"].ToString());
            info.AppendLine("정렬번호 : " + jsonData["index"].ToString());
            info.AppendLine();
            Debug.Log(info);
        }

        callback?.Invoke(bro);
    }
}
