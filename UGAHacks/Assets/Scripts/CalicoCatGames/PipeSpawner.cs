using UnityEngine;
using UnityEngine.SceneManagement; // ← ADDED

public class PipeSpawner : MonoBehaviour
{
    [Header("Timing")]
    public float spawnInterval = 2.0f;

    [Header("Pipe Movement")]
    public float pipeSpeed = 3f;

    [Header("Gap Settings")]
    public float gapSize = 3.0f;
    public float minGapY = -1.0f;
    public float maxGapY = 2.5f;

    [Header("Pipe Size")]
    public float pipeWidth = 1.5f;
    public float pipeHeight = 15f;

    [Header("Visuals")]
    public Color pipeColor = new Color(0.3f, 0.8f, 0.3f);

    private float timer;

    void Update()
    {
        if (!FlappyCatGame.Instance.IsPlaying) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPipePair();
            timer = 0f;
        }
    }

    void SpawnPipePair()
    {
        float gapY = Random.Range(minGapY, maxGapY);

        // ── Parent holds all parts and moves them together ──
        GameObject pair = new GameObject("PipePair");
        pair.transform.position = new Vector3(transform.position.x, 0f, 0f);

        // ★ MOVE TO THIS SCENE so it unloads with FlappyCatScene ★
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
            pair, gameObject.scene);

        Pipe pipe = pair.AddComponent<Pipe>();
        pipe.moveSpeed = pipeSpeed;

        // ── Bottom Pipe ──
        float bottomY = gapY - gapSize / 2f - pipeHeight / 2f;
        CreatePipeHalf(pair.transform, bottomY, "BottomPipe");

        // ── Top Pipe ──
        float topY = gapY + gapSize / 2f + pipeHeight / 2f;
        CreatePipeHalf(pair.transform, topY, "TopPipe");

        // ── Score Zone (invisible trigger in the gap) ──
        GameObject zone = new GameObject("ScoreZone");
        zone.transform.SetParent(pair.transform);
        zone.transform.localPosition = new Vector3(0f, gapY, 0f);

        BoxCollider2D trigger = zone.AddComponent<BoxCollider2D>();
        trigger.size = new Vector2(0.5f, gapSize);
        trigger.isTrigger = true;

        zone.AddComponent<ScoreZone>();
    }

    void CreatePipeHalf(Transform parent, float yPos, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = new Vector3(0f, yPos, 0f);
        obj.transform.localScale = new Vector3(pipeWidth, pipeHeight, 1f);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.Square;
        sr.color = pipeColor;
        sr.sortingOrder = 1;

        obj.AddComponent<BoxCollider2D>();
    }
}