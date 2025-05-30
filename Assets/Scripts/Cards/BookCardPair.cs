using UnityEngine;

[System.Serializable]
public class BookCardPair
{
    public string pairID;                 // 두 카드의 공통 ID
    public Sprite coverImage;            // 책 표지 이미지 (카드 1)
    public string keywordText;           // 책 키워드 텍스트 (카드 2)
}
