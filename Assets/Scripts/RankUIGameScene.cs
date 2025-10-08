using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using System.Collections;
using System.Collections.Generic;

public class RankUIGameScene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rankItemPrefab;
    public Transform contentParent;
    public Button refreshButton;
    public Button closeButton;
    public TextMeshProUGUI titleText;
    [Header("My Rank UI")]
    public TextMeshProUGUI RankText;
    public TextMeshProUGUI NicknameText;
    public TextMeshProUGUI ScoreText;

    private string rankUUID = "01993980-12fe-7756-b453-36dcb211d312"; // 뒤끝 콘솔 UUID

    void Start()
    {
        refreshButton.onClick.AddListener(LoadRanking);
        titleText.text = "RANKING";
    }

    public void Open()
    {
        gameObject.SetActive(true);
        LoadRanking();
        LoadMyRank();
    }

    void LoadRanking()
    {
        // 기존 목록 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 랭킹 데이터 요청
        var bro = Backend.URank.User.GetRankList(rankUUID);
        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 조회 실패: " + bro);
            return;
        }

        var rows = bro.FlattenRows();

        foreach (LitJson.JsonData row in rows)
        {
            GameObject item = Instantiate(rankItemPrefab, contentParent);
            item.transform.localScale = Vector3.one;

            string rank = "    " + row["rank"].ToString();
            string nickname = "        " + row["nickname"].ToString();
            string score = "     " + row["score"].ToString();

            item.transform.Find("RankText").GetComponent<TextMeshProUGUI>().text = rank;
            item.transform.Find("NicknameText").GetComponent<TextMeshProUGUI>().text = nickname;
            item.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score;
        }

        Debug.Log($"랭킹 데이터 {rows.Count}개 로드 완료");

        StartCoroutine(RefreshLayoutNextFrame());
    }

    private IEnumerator RefreshLayoutNextFrame()
    {
        yield return null; // 한 프레임 대기

        RectTransform rt = contentParent.GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        Canvas.ForceUpdateCanvases();

        rt.anchoredPosition = Vector2.zero;

        Debug.Log($"[AFTER LAYOUT FIX] Content anchoredPosition: {rt.anchoredPosition}, sizeDelta: {rt.sizeDelta}");
    }


    void LoadMyRank()
    {
        string myNickname = PlayerPrefs.GetString("StudentNumber", null); // 로그인 시 저장된 닉네임

        if (string.IsNullOrEmpty(myNickname))
        {
            Debug.LogWarning("PlayerPrefs에 nickname이 저장되어 있지 않습니다.");
            return;
        }

        var bro = Backend.URank.User.GetRankList(rankUUID, 100); // 상위 100명 조회
        if (!bro.IsSuccess())
        {
            Debug.LogError("랭킹 조회 실패: " + bro);
            return;
        }

        var rows = bro.FlattenRows();
        bool found = false;

        foreach (LitJson.JsonData row in rows)
        {
            string nickname = row["nickname"].ToString();
            if (nickname == myNickname)
            {
                GameObject myRankObj = GameObject.Find("MyRanking (1)");
                GameObject myNickObj = GameObject.Find("MyNickname (1)");
                GameObject myScoreObj = GameObject.Find("MyScore (1)");

                if (myRankObj != null)
                    myRankObj.GetComponent<TextMeshProUGUI>().text = row["rank"].ToString();

                if (myNickObj != null)
                    myNickObj.GetComponent<TextMeshProUGUI>().text = nickname;

                if (myScoreObj != null)
                    myScoreObj.GetComponent<TextMeshProUGUI>().text = row["score"].ToString();

                Debug.Log($"내 랭킹 UI 표시 완료 → {row["rank"]}위 | {nickname} | {row["score"]}");
                found = true;
                break;
            }
        }

        if (!found)
        {
            GameObject myRankObj = GameObject.Find("MyRanking (1)");
            GameObject myNickObj = GameObject.Find("MyNickname (1)");
            GameObject myScoreObj = GameObject.Find("MyScore (1)");

            if (myRankObj != null)
                myRankObj.GetComponent<TextMeshProUGUI>().text = "-";

            if (myNickObj != null)
                myNickObj.GetComponent<TextMeshProUGUI>().text = myNickname;

            if (myScoreObj != null)
                myScoreObj.GetComponent<TextMeshProUGUI>().text = "기록 없음";

            Debug.Log($"닉네임 '{myNickname}' 에 해당하는 랭킹을 찾지 못함.");
        }

        StartCoroutine(RefreshLayoutNextFrame());
    }

}
