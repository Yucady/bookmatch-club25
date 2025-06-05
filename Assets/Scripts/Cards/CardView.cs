using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class CardView : MonoBehaviour
{
    private GameObject front; // Front ��Ʈ
    private GameObject back;  // ���� Back

    private Image frontImage;         // Front/CoverImage
    private TMP_Text frontText;       // Front/KeywordText

    private string pairID;           // ��Ī Ȯ�ο� ID
    private bool isImageCard;        // �ո��� �̹����ΰ�?
    private bool isFlipping = false;

    private bool isMatched = false;
    public bool IsMatched => isMatched;

    private void Awake()
    {
        // �ڽ� ������Ʈ Ž��
        front = transform.Find("Front")?.gameObject;
        back = transform.Find("Back")?.gameObject;

        frontImage = front.transform.Find("Image")?.GetComponent<Image>(); // åǥ��
        frontText = front.transform.Find("Text/Text (TMP)")?.GetComponent<TMP_Text>(); // Ű����
    }

    /// <summary>
    /// ī�忡 ������ ����
    /// </summary>
    public void Setup(string id, bool isImage, Sprite image, string keyword) //ī�� ����
    {
        pairID = id;
        isImageCard = isImage;

        if (isImageCard)
        {
            frontImage.sprite = image;
            frontImage.gameObject.SetActive(true);
            frontText.gameObject.SetActive(false);
        }
        else
        {
            frontText.text = keyword;
            frontText.gameObject.SetActive(true);
            frontImage.gameObject.SetActive(false);
        }

        ShowBack(); // ī�� ������ �׻� �޸�
    }

    //public void OnClick()
    //{
    //    if (!isFlipping)
    //    {
    //        Debug.Log("OnClicked");
    //        Flip(); // Ŭ�� �� ī�� ������ ���� ����
    //        CardMatchController.Instance.SelectCard(this);
    //    }
    //}

    public void OnClick()
    {
        Debug.Log("OnClicked1");
        if (CardMatchController.Instance.IsInputLocked) return;

        Debug.Log("OnClicked2");
        CardMatchController.Instance.SelectCard(this);
    }


    public void SetMatched(bool value)
    {
        isMatched = value;
    }

    public void OnClickEffect()
    {
        transform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(0.9f, 0.1f))
            .Append(transform.DOScale(1f, 0.1f));
    }

    public void Flip()
    {
        if (isFlipping) return;
        isFlipping = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleX(0, 0.2f))
            .AppendCallback(() =>
            {
                CardMatchController.Instance.LockInput();
                bool showingFront = front.activeSelf;
                front.SetActive(!showingFront);
                back.SetActive(showingFront);
            })
            .Append(transform.DOScale(1, 0.2f))
            .OnComplete(() =>
            {
                CardMatchController.Instance.UnlockInput();
                isFlipping = false; 
            }); 
    }

    public void ShowBack()
    {
        front.SetActive(false);
        back.SetActive(true);
    }

    public void ShowFront()
    {
        front.SetActive(true);
        back.SetActive(false);
    }

    public string GetPairID()
    {
        return pairID;
    }

    public bool IsImageCard()
    {
        return isImageCard;
    }
}
