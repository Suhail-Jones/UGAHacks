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

    // Other scripts read these
    public bool IsPlaying { get; private set; }
    public int Score { get; private set; }

    private bool lastResult;

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

    // ── Countdown before gameplay begins ──
    IEnumerator CountdownRoutine()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "3";  yield return new WaitForSeconds(1f);
        messageText.text = "2";  yield return new WaitForSeconds(1f);
        messageText.text = "1";  yield return new WaitForSeconds(1f);
        messageText.text = "FLAP!";
        yield return new WaitForSeconds(0.5f);
        messageText.gameObject.SetActive(false);
        IsPlaying = true;
    }

    // ── Called by ScoreZone when cat clears a pipe gap ──
    public void AddScore()
    {
        if (!IsPlaying) return;
        Score++;
        UpdateScoreDisplay();
        if (Score >= scoreToWin) EndGame(true);
    }

    // ── Called by FlappyCat when it hits something ──
    public void GameOver()
    {
        if (!IsPlaying) return;
        EndGame(false);
    }

    void EndGame(bool won)
    {
        IsPlaying = false;
        lastResult = won;

        gameOverPanel.SetActive(true);
        messageText.gameObject.SetActive(true);
        messageText.text = won ? "PURRFECT!" : "GAME OVER!";

        StartCoroutine(AutoReturnRoutine());
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {Score} / {scoreToWin}";
    }

    // ── Return to main stage ──
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
        if (GameManager.Instance != null)
            GameManager.Instance.EndMinigameScene(lastResult);
        else
            Debug.LogWarning("GameManager not found!");
    }
}