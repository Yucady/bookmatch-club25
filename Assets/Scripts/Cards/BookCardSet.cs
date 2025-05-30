using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBookCardSet", menuName = "Card Matching/Book Card Set")]
public class BookCardSet : ScriptableObject
{
    public List<BookCardPair> bookPairs;
}
