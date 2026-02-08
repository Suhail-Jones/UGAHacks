using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class BreakoutGame : MonoBehaviour
{
    public static BreakoutGame Instance { get; private set; }

    [Header("Game Settings")]
    public int startingLives = 3;

    [Header("Auto-Return")]
    public float endDelay = 5f;

    [Header("Play Area")]
    public float areaWidth = 11f;
    public float areaHeight = 11f;
    public float wallThickness = 0.5f;

    [Header("Brick Grid")]
    public int columns = 8;
    public int rows = 4;
    public float brickWidth = 1.2f;
    public float brickHeight = 0.5f;
    public float brickSpacing = 0.1f;
    public float brickStartY = 3.5f;

    [Header("UI References (drag these in)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI messageText;

    [Header("Minigame Music")]
    public AudioSource musicSource;
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    public bool IsPlaying { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }

    private int bricksRemaining;
    private GameManager cachedGM;

    private readonly Color[] rowColors =
    {
        new Color(1f, 0.3f, 0.3f),
        new Color(1f, 0.6f, 0.2f),
        new Color(1f, 1f, 0.3f),
        new Color(0.3f, 1f, 0.3f),
        new Color(0.3f, 0.6f, 1f),
        new Color(0.7f, 0.3f, 1f),
    };

    void Awake() { Instance = this; }

    void Start()
    {
        cachedGM = GameManager.Instance;
        if (cachedGM == null)
        {
            GameManager[] all = Resources.FindObjectsOfTypeAll<GameManager>();
            if (all.Length > 0) cachedGM = all[0];
        }

        Score = 0;
        Lives = startingLives;
        IsPlaying = false;

        CreateWalls();
        CreateBricks();
        UpdateUI();
        StartCoroutine(CountdownRoutine());
    }

    // ===================== MUSIC =====================

    void StartMusic()
    {
        if (musicSource == null || bgmClip == null) return;

        musicSource.clip = bgmClip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    // ===================== WALLS & BRICKS =====================

    void CreateWalls()
    {
        float hw = areaWidth / 2f;
        float hh = areaHeight / 2f;
        float t = wallThickness;

        MakeWall(new Vector3(-hw - t / 2f, 0, 0),
                 new Vector3(t, areaHeight + t * 2, 1));
        MakeWall(new Vector3(hw + t / 2f, 0, 0),
                 new Vector3(t, areaHeight + t * 2, 1));
        MakeWall(new Vector3(0, hh + t / 2f, 0),
                 new Vector3(areaWidth + t * 2, t, 1));
    }

    void MakeWall(Vector3 pos, Vector3 scale)
    {
        GameObject w = new GameObject("Wall");
        w.transform.position = pos;
        w.transform.localScale = scale;

        SpriteRenderer sr = w.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteHelper.Square;
        sr.color = new Color(0.25f, 0.25f, 0.35f);
        sr.sortingOrder = 0;

        w.AddComponent<BoxCollider2D>();

        SceneManager.MoveGameObjectToScene(w, gameObject.scene);
    }

    void CreateBricks()
    {
        float totalW = columns * (brickWidth + brickSpacing) - brickSpacing;
        float startX = -totalW / 2f + brickWidth / 2f;

        bricksRemaining = 0;

        for (int row = 0; row < rows; row++)
        {
            Color color = rowColors[row % rowColors.Length];

            for (int col = 0; col < columns; col++)
            {
                float x = startX + col * (brickWidth + brickSpacing);
                float y = brickStartY - row * (brickHeight + brickSpacing);

                GameObject b = new GameObject($"Brick_{row}_{col}");
                b.transform.position = new Vector3(x, y, 0);
                b.transform.localScale = new Vector3(brickWidth, brickHeight, 1);

                SpriteRenderer sr = b.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteHelper.Square;
                sr.color = color;
                sr.sortingOrder = 2;

                b.AddComponent<BoxCollider2D>();
                b.AddComponent<Brick>();

                SceneManager.MoveGameObjectToScene(b, gameObject.scene);

                bricksRemaining++;
            }
        }
    }

    // ===================== GAMEPLAY =====================

    IEnumerator CountdownRoutine()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "3"; yield return new WaitForSeconds(1f);
        messageText.text = "2"; yield return new WaitForSeconds(1f);
        messageText.text = "1"; yield return new WaitForSeconds(1f);
        messageText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        messageText.gameObject.SetActive(false);

        IsPlaying = true;
        StartMusic();
    }

    public void BrickDestroyed()
    {
        Score += 10;
        bricksRemaining--;
        UpdateUI();

        if (bricksRemaining <= 0)
            EndGame(true);
    }

    public void LoseLife()
    {
        Lives--;
        UpdateUI();

        if (Lives <= 0)
            EndGame(false);
    }

    void EndGame(bool won)
    {
        IsPlaying = false;
        StopMusic();

        messageText.gameObject.SetActive(true);
        messageText.text = won ? "ALL CLEAR!" : "NO LIVES LEFT!";

        StartCoroutine(ReturnToStageSequence(won));
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {Score}";
        livesText.text = $"Lives: {Lives}";
    }

    // ===================== RETURN =====================

    IEnumerator ReturnToStageSequence(bool won)
    {
        yield return new WaitForSeconds(endDelay);
        ReturnToStage(won);
    }

    void ReturnToStage(bool playerWon)
    {
        float performance = playerWon ? 1f : 0f;

        if (cachedGM != null)
        {
            cachedGM.EndMinigameScene(performance);
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndMinigameScene(performance);
            return;
        }

        GameManager[] all = Resources.FindObjectsOfTypeAll<GameManager>();
        if (all.Length > 0)
        {
            all[0].EndMinigameScene(performance);
            return;
        }

        MinigameController controller = FindObjectOfType<MinigameController>();
        if (controller != null)
        {
            if (playerWon) controller.WinGame();
            else controller.LoseGame();
            return;
        }

        Debug.LogWarning("GameManager not found — reloading main scene directly.");
        SceneManager.LoadScene("Stage");
    }
}