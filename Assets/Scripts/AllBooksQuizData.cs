using UnityEngine;

[CreateAssetMenu(fileName = "AllBooksQuizData", menuName = "Quiz/AllBooksQuizData")]
public class AllBooksQuizData : ScriptableObject
{
    public BookQuizData[] books; // 18권 데이터
}
