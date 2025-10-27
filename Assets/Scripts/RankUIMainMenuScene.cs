using UnityEngine;
using TMPro;
using BackEnd;

public class RankUIMainMenuScene : MonoBehaviour
{
    [Header("랭킹 텍스트 UI (상위 3명)")]
    public TextMeshProUGUI rank1Text;
    public TextMeshProUGUI rank2Text;
    public TextMeshProUGUI rank3Text;

    // 뒤끝 콘솔에서 생성된 랭킹보드 UUID (예시)
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
        // 초기화 시 게스트 로그인부터 랭킹 표시까지 수행
        GuestLoginAndFetchRank();
    }

    private void GuestLoginAndFetchRank()
    {
        Debug.Log("게스트 로그인 시도 중...");

        // 기존 게스트 정보 삭제 (리턴값 없음)
        Backend.BMember.DeleteGuestInfo();
        Debug.Log("기존 게스트 정보 초기화 완료");

        // 새 게스트 로그인 시도
        var bro = Backend.BMember.GuestLogin();
        if (bro.IsSuccess())
        {
            Debug.Log("게스트 로그인 성공");
            FetchTop3Rank(); // 로그인 성공 시 랭킹 불러오기
        }
        else
        {
            Debug.LogError("게스트 로그인 실패: " + bro);
            Debug.LogWarning("해결 팁: 뒤끝 콘솔에서 유저 테이블/프로젝트 키 확인 필요");
        }
    }


    private void FetchTop3Rank()
    {
        Debug.Log("랭킹 조회 시도...");
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

    //FlattenRows()를 이용한 안전한 랭킹 표시
    private void ShowRankUIFlattened(BackendReturnObject rankList)
    {
        TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[] { rank1Text, rank2Text, rank3Text };
        int index = 0;

        foreach (LitJson.JsonData row in rankList.FlattenRows())
        {
            if (index >= rankTexts.Length) break;

            string nickname = row.ContainsKey("nickname") ? row["nickname"].ToString() : "-";
            float scoreValue = 0f;

            if (row.ContainsKey("score") && float.TryParse(row["score"].ToString(), out float result))
                scoreValue = result;

            rankTexts[index].text = $"{nickname.Trim()} - {scoreValue:F2}초";
            index++;
        }

        // 나머지 순위 칸 비우기
        for (; index < rankTexts.Length; index++)
        {
            rankTexts[index].text = " -";
        }
    }
}
