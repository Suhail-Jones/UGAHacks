using UnityEngine;

/// <summary>
/// Added automatically by PipeSpawner inside each pipe gap.
/// Awards a point when the Player passes through.
/// </summary>
public class ScoreZone : MonoBehaviour
{
    private bool scored = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // "Player" is a built-in Unity tag — no need to create it
        if (!scored && other.CompareTag("Player"))
        {
            scored = true;
            FlappyCatGame.Instance.AddScore();
        }
    }
}