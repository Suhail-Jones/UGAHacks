using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FlappyCatGame : MonoBehaviour
{
    public static FlappyCatGame Instance { get; private set; }

    [Header("Win Condition")]
    public int scoreToWin = 5;

    [Header("UI References (drag these in)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;
    public GameObject gameOverPanel;
    public Button continueButton;

    [Header("Minigame Music")]
    public AudioSource musicSource;
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    // Other scripts read these
    public bool IsPlaying { get; private set; }
    public int Score { get; private set; }

    void Awake() { Instance = this; }

    void Start()
    {
        Score = 0;
        IsPlaying = false;
        gameOverPanel.SetActive(false);
        UpdateScoreDisplay();

        if (continueButton != null)
            continueButton.onClick.AddListener(ManualReturn);

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

    // ===================== COUNTDOWN =====================

    IEnumerator CountdownRoutine()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "3"; yield return new WaitForSeconds(1f);
        messageText.text = "2"; yield return new WaitForSeconds(1f);
        messageText.text = "1"; yield return new WaitForSeconds(1f);
        messageText.text = "FLAP!";
        yield return new WaitForSeconds(0.5f);
        messageText.gameObject.SetActive(false);

        IsPlaying = true;
        StartMusic();
    }

    // ===================== GAMEPLAY =====================

    public void AddScore()
    {
        if (!IsPlaying) return;
        Score++;
        UpdateScoreDisplay();
        if (Score >= scoreToWin) EndGame(true);
    }

    public void GameOver()
    {
        if (!IsPlaying) return;
        EndGame(false);
    }

    void EndGame(bool won)
    {
        IsPlaying = false;
        StopMusic();

        gameOverPanel.SetActive(true);
        messageText.gameObject.SetActive(true);
        messageText.text = won ? "PURRFECT!" : "GAME OVER!";

        StartCoroutine(AutoReturnRoutine());
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {Score} / {scoreToWin}";
    }

    // ===================== RETURN =====================

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

    public float GetPerformance()
    {
        return Mathf.Clamp01((float)Score / scoreToWin);
    }
}