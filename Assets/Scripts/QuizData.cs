using System;

[System.Serializable]
public class QuizData
{
    public QuizType quizType; // 문제 유형 (OX, MultipleChoice, Subjective)
    public string question;   // 문제 텍스트
    public string answer;     // 정답

    // 1단계(OX) → answer는 "O" 또는 "X"
    // 2단계(객관식) → choices 배열 사용
    public string[] choices;  // 선택지(2단계만 사용)

    // 3단계(주관식) → 힌트 사용
    public string hint;       // 힌트 내용
    public float hintDelay;   // 힌트 표시 지연 시간 (초 단위)
}
