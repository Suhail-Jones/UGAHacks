using UnityEngine;

public class MinigameController : MonoBehaviour
{
    /// <summary>
    /// Call this with a 0.0–1.0 performance value.
    /// </summary>
    public void EndGame(float performance)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EndMinigameScene(performance);
    }

    // Legacy convenience methods (for any minigame that only knows win/lose)
    public void WinGame() => EndGame(1f);
    public void LoseGame() => EndGame(0f);
}