using UnityEngine;

public class BreakoutPaddle : MonoBehaviour
{
    [Header("Settings")]
    public float speed    = 10f;
    public float boundary = 4.5f;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
        {
            sr.sprite = SpriteHelper.Square;
            sr.color  = Color.white;
        }

        // ★ FIX: Manually set collider size so it doesn't fail on empty sprite ★
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size   = new Vector2(1f, 1f);
            col.offset = Vector2.zero;
        }
    }

    void Update()
    {
        if (BreakoutGame.Instance == null || !BreakoutGame.Instance.IsPlaying)
            return;

        float input = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += input * speed * Time.deltaTime;
        pos.x  = Mathf.Clamp(pos.x, -boundary, boundary);
        transform.position = pos;
    }
}