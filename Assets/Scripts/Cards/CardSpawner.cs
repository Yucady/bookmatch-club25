using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class CardSpawner : MonoBehaviour
{
    public BookCardSet cardSet; // ScriptableObject ����
    public GameObject cardPrefab; // ī�� ������
    public Transform cardParent; // ī����� �� �θ� (CardGrid)

    private void Start()
    {
        SpawnCards();
        StartCoroutine(RevealAllCards());
    }

    private IEnumerator RevealAllCards() // ���� ������
    {
        List<CardView> cards = FindObjectsByType<CardView>(FindObjectsSortMode.None).ToList();

        yield return new WaitForSeconds(1f);

        // ��ü �ո� �����ֱ�
        foreach (var card in cards)
            card.Flip();

        yield return new WaitForSeconds(3f); // �÷��̾ �� �ð� ����

        // �ٽ� �޸�����
        foreach (var card in cards)
            card.Flip();

        // ���� Ŭ�� ���
        CardMatchController.Instance.IsInputLocked = false;
    }


    void SpawnCards()  // ī�� ����
    {
        List<BookCardPair> pairs = new List<BookCardPair>(cardSet.bookPairs);
        if (pairs.Count < 6)
        {
            Debug.LogError("�� �����Ͱ� �����մϴ�! �ּ� 6�� �ʿ�.");
            return;
        }

        // 6�� �̱�
        List<BookCardPair> selectedPairs = pairs.OrderBy(x => Random.value).Take(6).ToList();

        // ī�� 12�� �����
        List<(string pairID, bool isImage, Sprite cover, string keyword)> cardInfos = new();

        foreach (var pair in selectedPairs)
        {
            // �̹��� ī�� �ϳ�
            cardInfos.Add((pair.pairID, true, pair.coverImage, null));
            // Ű���� ī�� �ϳ�
            cardInfos.Add((pair.pairID, false, null, pair.keywordText));
        }

        // ī�� ����
        cardInfos = cardInfos.OrderBy(x => Random.value).ToList(); // ���� ����

        foreach (var info in cardInfos)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardParent);
            CardView view = cardGO.GetComponent<CardView>();
            view.Setup(info.pairID, info.isImage, info.cover, info.keyword);
        }
    }
}

