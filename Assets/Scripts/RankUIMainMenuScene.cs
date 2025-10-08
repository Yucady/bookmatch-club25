using UnityEngine;
using TMPro;
using BackEnd;

public class RankUIMainMenuScene : MonoBehaviour
{
    [Header("랭킹 텍스트 UI (상위 3명)")]
    public TextMeshProUGUI rank1Text;
    public TextMeshProUGUI rank2Text;
    public TextMeshProUGUI rank3Text;

    private string rankUUID = "01993980-12fe-7756-b453-36dcb211d312";

    private void OnEnable()
    {
        BackendManager.OnBackendInitialized += InitializeRankUI;
    }

    private void OnDisable()
    {
        BackendManager.OnBackendInitialized -= InitializeRankUI;
    }

    private void InitializeRankUI()
    {
        GuestLoginAndFetchRank();
    }

    private void GuestLoginAndFetchRank()
    {
        var bro = Backend.BMember.GuestLogin();
        if (bro.IsSuccess())
        {
            Debug.Log("게스트 로그인 성공");
            FetchTop3Rank();
        }
        else
        {
            Debug.LogError("게스트 로그인 실패: " + bro);
        }
    }

    private void FetchTop3Rank()
    {
        var bro = Backend.URank.User.GetRankList(rankUUID, 3); // 상위 3명 조회
        if (bro.IsSuccess())
        {
            Debug.Log("랭킹 조회 성공");
            ShowRankUIFlattened(bro);
        }
        else
        {
            Debug.LogError("랭킹 조회 실패: " + bro);
        }
    }

    // FlattenRows()를 이용한 안전한 랭킹 표시
    private void ShowRankUIFlattened(BackendReturnObject rankList)
    {
        TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[] { rank1Text, rank2Text, rank3Text };
        int index = 0;

        foreach (LitJson.JsonData row in rankList.FlattenRows())
        {
            if (index >= rankTexts.Length) break;

            string nickname = row["nickname"]?.ToString() ?? "-";

            // score를 float로 변환 후 소수점 2자리까지 표시
            float scoreValue = 0f;
            if (row["score"] != null && float.TryParse(row["score"].ToString(), out float result))
            {
                scoreValue = result;
            }

            rankTexts[index].text = $" {nickname.Trim()} - {scoreValue:F2}초";
            index++;
        }

        // 남은 텍스트는 비워주기
        for (; index < rankTexts.Length; index++)
        {
            rankTexts[index].text = " -";
        }
    }

}
