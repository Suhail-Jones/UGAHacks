using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClickGameManager : MonoBehaviour
{
    public static ClickGameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 15f;
    public int targetScore = 15; // Score needed to "Win" this interaction
    public float buttonPadding = 80f; // pixels from screen edge
    public float endDelay = 5f; // Time to wait before returning to stage

    [Header("UI References")]
    public Button clickButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverPanel;
    public Button restartButton; // This now acts as the "Continue/Done" button

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

        // Setup the Continue Button to skip the 5s wait
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(ManualReturn);
            
            TextMeshProUGUI btnText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
            if(btnText != null) btnText.text = "CONTINUE";
        }

        StartGame();
    }

    // --- ADDED THIS BACK SO YOUR COUNTDOWN WORKS ---
    public void SetPlaying(bool value)
    {
        IsPlaying = value;
        if (value)
        {
            clickButton.interactable = true;
        }
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
        
        // Check if we hit the target
        string result = Score >= targetScore ? "SUCCESS!" : "FAILED";
        finalScoreText.text = $"{result}\nScore: {Score} / {targetScore}";
        
        // Wait 5 seconds before leaving
        StartCoroutine(ReturnToStageSequence());
    }
    
    // Allows the player to click "Continue" to skip the 5s wait
    void ManualReturn()
    {
        StopAllCoroutines();
        ReturnToStage();
    }

    IEnumerator ReturnToStageSequence()
    {
        yield return new WaitForSeconds(endDelay);
        ReturnToStage();
    }

    // --- FUNCTION TO GO BACK TO STAGE ---
    void ReturnToStage()
    {
        bool playerWon = Score >= targetScore;

        // 1. Try to find the Minigame Controller
        MinigameController controller = FindObjectOfType<MinigameController>();
        
        if (controller != null)
        {
            if (playerWon) controller.WinGame();
            else controller.LoseGame();
        }
        else
        {
            // 2. Fail-Safe: If MinigameController is missing, talk to GameManager directly
            Debug.LogWarning("MinigameController not found! Calling GameManager directly.");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndMinigameScene(playerWon);
            }
        }
    }
}