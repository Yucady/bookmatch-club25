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

    public void SelectCard(CardView selected) // 카드 매칭(첫카드, 두번째카드 / Flip)
    {
        if (secondSelected != null) return;
        if (firstSelected == selected) return; // 같은 카드 다시 클릭 → 무시

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
        yield return new WaitForSeconds(1f); // Flip 애니메이션 기다림

        if (firstSelected.GetPairID() == secondSelected.GetPairID())
        {
            // 매치 성공
            Debug.Log("Matched!");
            firstSelected = null;
            secondSelected = null;
        }
        else
        {
            // 매치 실패 → 다시 뒤집기
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
            Debug.Log("게임 클리어!");
            // 추가 처리 (예: UI 표시, 리스타트 버튼 등)
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
