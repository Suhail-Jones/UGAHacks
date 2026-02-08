using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BreakoutGame : MonoBehaviour
{
    public static BreakoutGame Instance { get; private set; }

    [Header("Game Settings")]
    public int startingLives = 3;

    [Header("Play Area")]
    public float areaWidth  = 11f;
    public float areaHeight = 11f;
    public float wallThickness = 0.5f;

    [Header("Brick Grid")]
    public int   columns      = 8;
    public int   rows         = 4;
    public float brickWidth   = 1.2f;
    public float brickHeight  = 0.5f;
    public float brickSpacing = 0.1f;
    public float brickStartY  = 3.5f; // Y position of first row

    [Header("UI References (drag these in)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI messageText;
    public GameObject gameOverPanel;
    public Button continueButton;

    public bool IsPlaying { get; private set; }
    public int  Score     { get; private set; }
    public int  Lives     { get; private set; }

    private int  bricksRemaining;
    private bool lastResult;

    // Row colors (cycles if more rows than colors)
    private readonly Color[] rowColors =
    {
        new Color(1f, 0.3f, 0.3f),   // Red
        new Color(1f, 0.6f, 0.2f),   // Orange
        new Color(1f, 1f, 0.3f),     // Yellow
        new Color(0.3f, 1f, 0.3f),   // Green
        new Color(0.3f, 0.6f, 1f),   // Blue
        new Color(0.7f, 0.3f, 1f),   // Purple
    };

    void Awake() { Instance = this; }

    void Start()
    {
        Score = 0;
        Lives = startingLives;
        IsPlaying = false;
        gameOverPanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(ManualReturn);

        CreateWalls();
        CreateBricks();
        UpdateUI();
        StartCoroutine(CountdownRoutine());
    }

    // ── Builds the left, right, and top wall colliders ──
    void CreateWalls()
    {
        float hw = areaWidth  / 2f;
        float hh = areaHeight / 2f;
        float t  = wallThickness;

        // Left wall
        MakeWall(new Vector3(-hw - t / 2f, 0, 0),
                 new Vector3(t, areaHeight + t * 2, 1));
        // Right wall
        MakeWall(new Vector3( hw + t / 2f, 0, 0),
                 new Vector3(t, areaHeight + t * 2, 1));
        // Top wall
        MakeWall(new Vector3(0, hh + t / 2f, 0),
                 new Vector3(areaWidth + t * 2, t, 1));
    }

    void MakeWall(Vector3 pos, Vector3 scale)
    {
        GameObject w = new GameObject("Wall");
        w.transform.position   = pos;
        w.transform.localScale = scale;

        SpriteRenderer sr = w.AddComponent<SpriteRenderer>();
        sr.sprite       = SpriteHelper.Square;
        sr.color        = new Color(0.25f, 0.25f, 0.35f);
        sr.sortingOrder = 0;

        w.AddComponent<BoxCollider2D>();
    }

    // ── Generates the colored brick grid ──
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
                b.transform.position   = new Vector3(x, y, 0);
                b.transform.localScale = new Vector3(brickWidth, brickHeight, 1);

                SpriteRenderer sr = b.AddComponent<SpriteRenderer>();
                sr.sprite       = SpriteHelper.Square;
                sr.color        = color;
                sr.sortingOrder = 2;

                b.AddComponent<BoxCollider2D>();
                b.AddComponent<Brick>();

                bricksRemaining++;
            }
        }
    }

    IEnumerator CountdownRoutine()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "3";  yield return new WaitForSeconds(1f);
        messageText.text = "2";  yield return new WaitForSeconds(1f);
        messageText.text = "1";  yield return new WaitForSeconds(1f);
        messageText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        messageText.gameObject.SetActive(false);
        IsPlaying = true;
    }

    // ── Called by Brick.cs when a brick is destroyed ──
    public void BrickDestroyed()
    {
        Score += 10;
        bricksRemaining--;
        UpdateUI();

        if (bricksRemaining <= 0)
            EndGame(true);
    }

    // ── Called by BreakoutBall when ball falls off bottom ──
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
        lastResult = won;

        gameOverPanel.SetActive(true);
        messageText.gameObject.SetActive(true);
        messageText.text = won ? "ALL CLEAR!" : "NO LIVES LEFT!";

        StartCoroutine(AutoReturnRoutine());
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {Score}";
        livesText.text = $"Lives: {Lives}";
    }

    IEnumerator AutoReturnRoutine()
    {
        yield return new WaitForSeconds(3f);
        ReturnToStage();
    }

    void ManualReturn()
    {
        StopAllCoroutines();
        ReturnToStage();
    }

    void ReturnToStage()
    {
        float performance = GetPerformance();

        if (GameManager.Instance != null)
            GameManager.Instance.EndMinigameScene(performance);
        else
            Debug.LogWarning("GameManager not found!");
    }

    /// <summary>
    /// Performance based on lives remaining and bricks cleared.
    /// 1.0 = all bricks destroyed with all lives intact.
    /// 0.0 = lost all lives with no bricks destroyed.
    /// </summary>
    public float GetPerformance()
    {
        int totalBricks = columns * rows;

        // How many bricks were destroyed
        int bricksDestroyed = totalBricks - bricksRemaining;
        float clearRatio = totalBricks > 0 ? (float)bricksDestroyed / totalBricks : 0f;

        // How many lives survived
        float lifeRatio = startingLives > 0 ? (float)Lives / startingLives : 0f;

        // Weighted: clearing bricks matters most, surviving lives is a bonus
        float performance = (clearRatio * 0.7f) + (lifeRatio * 0.3f);

        return Mathf.Clamp01(performance);
    }
}