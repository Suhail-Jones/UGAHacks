using UnityEngine;

public class Note : MonoBehaviour
{
    public NoteData Data { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Vector3 startPos;
    private Vector3 endPos;
    private float travelTime;
    private float spawnSongTime;

    public void Initialize(NoteData data, Vector3 targetPos, float travel)
    {
        Data = data;
        endPos = targetPos;
        startPos = transform.position;
        travelTime = travel;
        spawnSongTime = data.hitTime - travel;
        IsActive = true;
    }

    void Update()
    {
        if (!IsActive) return;

        float songTime = SceneManager1.Instance.GetCurrentSongTime();
        float elapsed = songTime - spawnSongTime;
        float t = elapsed / travelTime;

        transform.position = Vector3.Lerp(startPos, endPos, t);

        // If the note has gone well past the hit zone, count as miss
        if (t > 1f + (SceneManager1.Instance.missWindow / travelTime) * 2f)
        {
            Miss();
        }
    }

    public void Hit()
    {
        if (!IsActive) return;
        IsActive = false;

        // Quick scale-down animation then destroy
        Destroy(gameObject);
    }

    public void Miss()
    {
        if (!IsActive) return;
        IsActive = false;

        SceneManager1.Instance.RegisterMiss();

        // Fade out then destroy
        Destroy(gameObject, 0.3f);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.3f);
    }
}