using UnityEngine;

/// <summary>
/// Added automatically by PipeSpawner. Moves the pipe pair to the left
/// and destroys it when it goes off-screen.
/// </summary>
public class Pipe : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        // Stop moving when the game isn't playing (countdown or game over)
        if (FlappyCatGame.Instance == null || !FlappyCatGame.Instance.IsPlaying)
            return;

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < -12f)
            Destroy(gameObject);
    }
}