using System.Collections;
using UnityEngine;
using System.Linq;


public class CardMatchController : MonoBehaviour
{
    public static CardMatchController Instance;

    private CardView firstSelected;
    private CardView secondSelected;

    public bool IsInputLocked = true;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectCard(CardView selected) // ī�� ��Ī(ùī��, �ι�°ī�� / Flip)
    {
        if (secondSelected != null) return;
        if (firstSelected == selected) return; // ���� ī�� �ٽ� Ŭ�� �� ����

        if (firstSelected == null)
        {
            firstSelected = selected;
            firstSelected.Flip();
            Debug.Log("isFlipped 1");
        }
        else
        {
            secondSelected = selected;
            secondSelected.Flip();

            Debug.Log("isFlipped 2");

            LockInput();
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f); // Flip �ִϸ��̼� ��ٸ�

        if (firstSelected.GetPairID() == secondSelected.GetPairID())
        {
            // ��ġ ����
            Debug.Log("Matched!");
            firstSelected = null;
            secondSelected = null;
        }
        else
        {
            // ��ġ ���� �� �ٽ� ������
            firstSelected.Flip();
            secondSelected.Flip();
            firstSelected = null;
            secondSelected = null;
        }

        UnlockInput();
        CheckGameClear();
    }

    private void CheckGameClear()
    {
        bool allMatched = FindObjectsByType<CardView>(FindObjectsSortMode.None)
                          .All(c => c.IsMatched);

        if (allMatched)
        {
            Debug.Log("���� Ŭ����!");
            // �߰� ó�� (��: UI ǥ��, ����ŸƮ ��ư ��)
        }
    }

    public void UnlockInput()
    {
        IsInputLocked = false;
    }

    public void LockInput()
    {
        IsInputLocked = true;
    }
}
