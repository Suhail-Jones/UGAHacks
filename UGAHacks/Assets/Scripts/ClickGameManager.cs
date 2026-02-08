using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ClickGameManager : MonoBehaviour
{
    public static ClickGameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 15f;
    public int targetScore = 15;
    public float buttonPadding = 80f;
    public float endDelay = 5f;

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

    [Header("Minigame Music")]
    public AudioSource musicSource;
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

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
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(ManualReturn);

            TextMeshProUGUI btnText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = "CONTINUE";
        }

        StartGame();
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

    // ===================== GAMEPLAY =====================

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

        StartMusic();
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
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 buttonSize = buttonRect.sizeDelta;

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
        timerText.color = TimeRemaining <= 3f ? Color.red : Color.white;
    }

    public float GetPerformance()
    {
        return Mathf.Clamp01((float)Score / targetScore);
    }

    void EndGame()
    {
        IsPlaying = false;
        StopMusic();

        clickButton.interactable = false;
        clickButton.gameObject.SetActive(false);

        float performance = GetPerformance();

        gameOverPanel.SetActive(true);
        gameOverText.text = "TIME'S UP!";

        string result = performance >= 1f ? "SUCCESS!" : "FAILED";
        finalScoreText.text = $"{result}\nScore: {Score} / {targetScore}";

        StartCoroutine(ReturnToStageSequence());
    }

    // ===================== RETURN =====================

    void ManualReturn()
    {
        StopAllCoroutines();
        StopMusic();
        ReturnToStage();
    }

    IEnumerator ReturnToStageSequence()
    {
        yield return new WaitForSeconds(endDelay);
        ReturnToStage();
    }

    void ReturnToStage()
    {
        float performance = GetPerformance();

        MinigameController controller = FindObjectOfType<MinigameController>();

        if (controller != null)
        {
            controller.EndGame(performance);
        }
        else
        {
            Debug.LogWarning("MinigameController not found! Calling GameManager directly.");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndMinigameScene(performance);
            }
        }
    }
}