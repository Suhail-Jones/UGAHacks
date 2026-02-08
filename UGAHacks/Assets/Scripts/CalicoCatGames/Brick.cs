using UnityEngine;

/// <summary>
/// Added automatically by BreakoutGame for each brick.
/// When the ball hits it, it tells the manager and destroys itself.
/// </summary>
public class Brick : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BreakoutBall>() != null)
        {
            BreakoutGame.Instance.BrickDestroyed();
            Destroy(gameObject);
        }
    }
}