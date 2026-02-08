using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public float timeLimit = 15f;
    private bool gameActive = true;
    public bool isScoreBased = false; 

    void Update() {
        if(!gameActive) return;
        
        timeLimit -= Time.deltaTime;
        
        // Survival Mode (Pong/Flappy): Win if time runs out
        if(!isScoreBased && timeLimit <= 0) {
            WinGame();
        }
        // Score Mode (Clicker): Lose if time runs out
        else if (isScoreBased && timeLimit <= 0) {
            LoseGame();
        }
    }

    public void WinGame()
    {
        gameActive = false;
        if(GameManager.Instance != null) GameManager.Instance.EndMinigameScene(true);
    }

    public void LoseGame()
    {
        gameActive = false;
        if(GameManager.Instance != null) GameManager.Instance.EndMinigameScene(false);
    }
}