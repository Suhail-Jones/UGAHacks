using UnityEngine;

/// <summary>
/// The calico cat! Press SPACE or LEFT-CLICK to flap upward.
/// Requires: Rigidbody2D, CircleCollider2D, SpriteRenderer.
/// Tag this GameObject as "Player".
/// </summary>
public class FlappyCat : MonoBehaviour
{
    [Header("Flap Settings")]
    public float flapForce = 7f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private bool gameStarted = false;

    void Awake()
    {
        // Auto-assign a placeholder sprite if none is set
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            sr.sprite = SpriteHelper.Circle;
            sr.color = new Color(1f, 0.7f, 0.3f); // Calico orange
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;      // Don't fall during countdown
        rb.freezeRotation = true;  // Keep cat upright
    }

    void Update()
    {
        if (isDead) return;
        if (!FlappyCatGame.Instance.IsPlaying) return;

        // Enable gravity on the very first gameplay frame
        if (!gameStarted)
        {
            gameStarted = true;
            rb.gravityScale = 2.5f;
        }

        // ── FLAP ──
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            rb.linearVelocity = new Vector2(0f, flapForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit a pipe or the ground → game over
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // Freeze the cat in place
        FlappyCatGame.Instance.GameOver();
    }
}