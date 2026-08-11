using UnityEngine;
using TMPro;

public class Scoring : MonoBehaviour
{
    private GameManager gameManager;
    private TMP_Text scoreText;
    private int currentScore;
    private int bestScore;
    private int totalScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentScore = 0; //Оставить
        bestScore = 0; //Загрузить лучший счет
        totalScore = 0; //Загрузить общий счет
        scoreText = GetComponentInChildren<TMP_Text>();
        gameManager = Camera.main.gameObject.GetComponent<GameManager>();
    }

    public void UpdateScore()
    {
        currentScore++;
        scoreText.text = currentScore.ToString();
       
    }
    public int GetScore()
    {
        return currentScore;
    }
    private void BestScore()
    {
        if (bestScore < currentScore)
        {
            bestScore = currentScore;
            //Сохранить лучший счет!
        }
    }
    private void ClearCurrentScore()
    {
        totalScore += currentScore;
        //Не забыть сохранить общий счет в дальнейшем!
        currentScore = 0;
    }

    private void OnEnable()
    {
        EventBus.isPlayerDead += BestScore;
        EventBus.isPlayerSurrender += ClearCurrentScore;
    }
    private void OnDisable()
    {
        EventBus.isPlayerDead -= BestScore;
        EventBus.isPlayerSurrender -= ClearCurrentScore;
    }
}
