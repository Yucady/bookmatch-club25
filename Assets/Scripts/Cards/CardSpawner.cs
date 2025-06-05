using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class CardSpawner : MonoBehaviour
{
    public BookCardSet cardSet; // ScriptableObject 참조
    public GameObject cardPrefab; // 카드 프리팹
    public Transform cardParent; // 카드들이 들어갈 부모 (CardGrid)

    private void Start()
    {
        SpawnCards();
        StartCoroutine(RevealAllCards());
    }

    private IEnumerator RevealAllCards() // 시작 뒤집기
    {
        List<CardView> cards = FindObjectsByType<CardView>(FindObjectsSortMode.None).ToList();

        yield return new WaitForSeconds(1f);

        // 전체 앞면 보여주기
        foreach (var card in cards)
            card.Flip();

        yield return new WaitForSeconds(3f); // 플레이어가 볼 시간 제공

        // 다시 뒷면으로
        foreach (var card in cards)
            card.Flip();

        // 이제 클릭 허용
        CardMatchController.Instance.IsInputLocked = false;
    }


    void SpawnCards()  // 카드 스폰
    {
        List<BookCardPair> pairs = new List<BookCardPair>(cardSet.bookPairs);
        if (pairs.Count < 6)
        {
            Debug.LogError("쌍 데이터가 부족합니다! 최소 6쌍 필요.");
            return;
        }

        // 6쌍 뽑기
        List<BookCardPair> selectedPairs = pairs.OrderBy(x => Random.value).Take(6).ToList();

        // 카드 12장 만들기
        List<(string pairID, bool isImage, Sprite cover, string keyword)> cardInfos = new();

        foreach (var pair in selectedPairs)
        {
            // 이미지 카드 하나
            cardInfos.Add((pair.pairID, true, pair.coverImage, null));
            // 키워드 카드 하나
            cardInfos.Add((pair.pairID, false, null, pair.keywordText));
        }

        // 카드 섞기
        cardInfos = cardInfos.OrderBy(x => Random.value).ToList(); // 랜덤 정렬

        foreach (var info in cardInfos)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardParent);
            CardView view = cardGO.GetComponent<CardView>();
            view.Setup(info.pairID, info.isImage, info.cover, info.keyword);
        }
    }
}

