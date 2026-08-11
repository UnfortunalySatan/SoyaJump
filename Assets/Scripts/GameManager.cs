using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathScreenUI;
    [Header("Score")]
    [SerializeField] private Scoring scoring;
    [Header("Another")]
    [SerializeField] private GameObject ground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        HideGround();
    }
    

    private int TakeScore()
    {
        int score = scoring.GetScore();
        return score;
    }
    public void StartGame()
    {
        deathScreenUI.SetActive(false);
        ground.SetActive(true);
    }
    public void PauseGame()
    {
        EventBus.isPause?.Invoke();
    }
    public void ResumeGame()
    {
        EventBus.isResume?.Invoke();
    }
    public void Death()
    {
        deathScreenUI.SetActive(true);
    }

    public void ContinueGame()
    {
        EventBus.isPlayerContinue?.Invoke();
    }
    public void Surrender()
    {
        EventBus.isPlayerSurrender?.Invoke();
        ShowGround();
    }
    public void ExitTheGame()
    {
        Application.Quit();
    }

    private void HideGround()
    {
        if (TakeScore() >= 50)
        {
            ground.SetActive(false);
        }
    }
    private void ShowGround()
    {
        ground.SetActive(true);
    }

    private void OnEnable()
    {
        EventBus.isPlayerDead += Death;
    }
    private void OnDisable()
    {
        EventBus.isPlayerDead -= Death;
    }
}
