using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClickGameManager : MonoBehaviour
{
    public static ClickGameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 15f;
    public float buttonPadding = 80f; // pixels from screen edge

    [Header("UI References")]
    public Button clickButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;
    public Button restartButton;

    [Header("Button Animation")]
    public float clickScalePunch = 1.4f;
    public float animationSpeed = 8f;

    // State
    public int Score { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsPlaying { get; private set; }

    private RectTransform buttonRect;
    private RectTransform canvasRect;
    private Vector3 buttonOriginalScale;
    private bool isAnimating;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        buttonRect = clickButton.GetComponent<RectTransform>();
        canvasRect = clickButton.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        buttonOriginalScale = buttonRect.localScale;

        clickButton.onClick.AddListener(OnButtonClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        StartGame();
    }

    void Update()
    {
        if (!IsPlaying) return;

        TimeRemaining -= Time.deltaTime;
        TimeRemaining = Mathf.Max(0f, TimeRemaining);
        UpdateTimerDisplay();

        if (TimeRemaining <= 0f)
        {
            EndGame();
        }

        // Animate button scale back to normal
        if (isAnimating)
        {
            buttonRect.localScale = Vector3.Lerp(
                buttonRect.localScale,
                buttonOriginalScale,
                Time.deltaTime * animationSpeed
            );

            if (Vector3.Distance(buttonRect.localScale, buttonOriginalScale) < 0.01f)
            {
                buttonRect.localScale = buttonOriginalScale;
                isAnimating = false;
            }
        }
    }

    public void StartGame()
    {
        Score = 0;
        TimeRemaining = gameDuration;
        IsPlaying = true;

        gameOverPanel.SetActive(false);
        clickButton.gameObject.SetActive(true);
        clickButton.interactable = true;

        UpdateScoreDisplay();
        UpdateTimerDisplay();
        MoveButtonToRandomPosition();
    }

    void OnButtonClicked()
    {
        if (!IsPlaying) return;

        Score++;
        UpdateScoreDisplay();
        PlayClickAnimation();
        MoveButtonToRandomPosition();
    }

    void MoveButtonToRandomPosition()
    {
        // Get canvas size
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 buttonSize = buttonRect.sizeDelta;

        // Calculate safe bounds (so the button stays fully on screen)
        float halfW = (buttonSize.x / 2f) + buttonPadding;
        float halfH = (buttonSize.y / 2f) + buttonPadding;

        float minX = -canvasSize.x / 2f + halfW;
        float maxX = canvasSize.x / 2f - halfW;
        float minY = -canvasSize.y / 2f + halfH;
        float maxY = canvasSize.y / 2f - halfH;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        buttonRect.anchoredPosition = new Vector2(randomX, randomY);
    }

    void PlayClickAnimation()
    {
        buttonRect.localScale = buttonOriginalScale * clickScalePunch;
        isAnimating = true;
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Clicks: {Score}";
    }

    void UpdateTimerDisplay()
    {
        timerText.text = $"Time: {TimeRemaining:F1}s";

        // Flash red when low
        if (TimeRemaining <= 3f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }

    void EndGame()
    {
        IsPlaying = false;
        clickButton.interactable = false;
        clickButton.gameObject.SetActive(false);

        gameOverPanel.SetActive(true);
        gameOverText.text = "TIME'S UP!";
        finalScoreText.text = $"Final Score: {Score}";
    }

    public void RestartGame()
    {
        buttonRect.localScale = buttonOriginalScale;
        isAnimating = false;
        StartGame();
    }

    public void SetPlaying(bool value)
    {
        IsPlaying = value;
    }
}