using UnityEngine;

[CreateAssetMenu(fileName = "BookQuizData", menuName = "Quiz/BookQuizData")]
public class BookQuizData : ScriptableObject
{
    public string bookTitle;       // 책 제목
    public string quizCreator;
    public Sprite coverImage;      // 책 표지 이미지
    public QuizData[] stage1;      // 1단계 문제 2개
    public QuizData[] stage2;      // 2단계 문제 2개
    public QuizData[] stage3;      // 3단계 문제 2개
}
