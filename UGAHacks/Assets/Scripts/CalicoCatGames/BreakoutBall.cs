using UnityEngine;

/// <summary>
/// Sits on the paddle until Space/Click is pressed, then launches upward.
/// Uses a bouncy PhysicsMaterial2D (created in code) to bounce off walls/bricks.
/// Requires: Rigidbody2D, CircleCollider2D, SpriteRenderer.
/// </summary>
public class BreakoutBall : MonoBehaviour
{
    [Header("Settings")]
    public float speed    = 8f;
    public float maxAngle = 60f;

    private Rigidbody2D rb;
    private bool launched = false;
    private Transform paddleTransform;
    private float paddleTopOffset = 0.5f;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            sr.sprite = SpriteHelper.Circle;
            sr.color  = Color.white;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ── Create a perfectly bouncy physics material in code ──
        PhysicsMaterial2D mat = new PhysicsMaterial2D("Bouncy");
        mat.bounciness = 1f;
        mat.friction   = 0f;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null) col.sharedMaterial = mat;

        // ── Rigidbody settings ──
        rb.gravityScale = 0f;
        rb.linearDamping         = 0f;
        rb.angularDamping  = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.isKinematic  = true;

        // Find the paddle
        BreakoutPaddle paddle = FindObjectOfType<BreakoutPaddle>();
        if (paddle != null) paddleTransform = paddle.transform;
    }

    void Update()
    {
        if (!launched)
        {
            // Follow the paddle
            if (paddleTransform != null)
                transform.position = paddleTransform.position
                                   + Vector3.up * paddleTopOffset;

            // Launch when player presses Space or clicks
            if (BreakoutGame.Instance != null
                && BreakoutGame.Instance.IsPlaying
                && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
            {
                Launch();
            }
            return;
        }

        // ── Safety: reset ball if it falls off screen ──
        if (transform.position.y < -7f)
        {
            BreakoutGame.Instance.LoseLife();
            ResetBall();
        }
    }

    void FixedUpdate()
    {
        if (!launched) return;
        if (rb.linearVelocity.sqrMagnitude < 0.01f) return;

        // ── Keep speed constant ──
        rb.linearVelocity = rb.linearVelocity.normalized * speed;

        // ── Prevent boring horizontal bouncing ──
        if (Mathf.Abs(rb.linearVelocity.y) < 1f)
        {
            float sign = rb.linearVelocity.y >= 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, sign * 1f).normalized * speed;
        }
    }

    void Launch()
    {
        launched = true;
        rb.isKinematic = false;

        float angle = Random.Range(-30f, 30f) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        rb.linearVelocity = dir.normalized * speed;
    }

    public void ResetBall()
    {
        launched = false;
        rb.linearVelocity        = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic     = true;

        if (paddleTransform != null)
            transform.position = paddleTransform.position
                               + Vector3.up * paddleTopOffset;
    }

    // ── Custom deflection off the paddle ──
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<BreakoutPaddle>() != null)
        {
            float hit  = transform.position.x - col.transform.position.x;
            float half = col.collider.bounds.size.x / 2f;
            float t    = Mathf.Clamp(hit / half, -1f, 1f);

            float angle = t * maxAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            rb.linearVelocity = dir.normalized * speed;
        }
    }
}