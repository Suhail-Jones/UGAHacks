using UnityEngine;

/// <summary>
/// Move with LEFT/RIGHT arrows or A/D keys.
/// Requires: Rigidbody2D (Kinematic), BoxCollider2D, SpriteRenderer.
/// </summary>
public class BreakoutPaddle : MonoBehaviour
{
    [Header("Settings")]
    public float speed    = 10f;
    public float boundary = 4.5f; // Max X the paddle center can reach

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            sr.sprite = SpriteHelper.Square;
            sr.color  = Color.white;
        }
    }

    void Update()
    {
        if (BreakoutGame.Instance == null || !BreakoutGame.Instance.IsPlaying)
            return;

        float input = Input.GetAxis("Horizontal"); // Arrow keys / A & D

        Vector3 pos = transform.position;
        pos.x += input * speed * Time.deltaTime;
        pos.x  = Mathf.Clamp(pos.x, -boundary, boundary);
        transform.position = pos;
    }
}