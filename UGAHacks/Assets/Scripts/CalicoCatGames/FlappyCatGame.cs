using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FlappyCatGame : MonoBehaviour
{
    public static FlappyCatGame Instance { get; private set; }

    [Header("Win Condition")]
    public int scoreToWin = 5;

    [Header("Auto-Return")]
    public float endDelay = 5f;

    [Header("UI References (drag these in)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;

    public bool IsPlaying { get; private set; }
    public int  Score     { get; private set; }

    void Awake() { Instance = this; }

    void Start()
    {
        Score = 0;
        IsPlaying = false;
        UpdateScoreDisplay();
        StartCoroutine(CountdownRoutine());
    }

    // ── Countdown before gameplay begins ──
    IEnumerator CountdownRoutine()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "3";    yield return new WaitForSeconds(1f);
        messageText.text = "2";    yield return new WaitForSeconds(1f);
        messageText.text = "1";    yield return new WaitForSeconds(1f);
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

        messageText.gameObject.SetActive(true);
        messageText.text = won ? "PURRFECT!" : "GAME OVER!";

        StartCoroutine(ReturnToStageSequence(won));
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {Score} / {scoreToWin}";
    }

    // ── Wait, then exit back to main scene ──
    IEnumerator ReturnToStageSequence(bool won)
    {
        yield return new WaitForSeconds(endDelay);
        ReturnToStage(won);
    }

void ReturnToStage(bool playerWon)
{
    // 1. Primary: Use GameManager directly (lives in the main scene)
    if (GameManager.Instance != null)
    {
        GameManager.Instance.EndMinigameScene(playerWon);
        return;
    }

    // 2. Fallback: Try MinigameController if GameManager isn't available
    MinigameController controller = FindObjectOfType<MinigameController>();
    if (controller != null)
    {
        float performance = GetPerformance();

        if (GameManager.Instance != null)
            GameManager.Instance.EndMinigameScene(performance);
        else
            Debug.LogWarning("GameManager not found!");
    }

    /// <summary>
    /// 0.0 = no pipes cleared, 1.0 = hit or exceeded the target.
    /// </summary>
    public float GetPerformance()
    {
        return Mathf.Clamp01((float)Score / scoreToWin);
    }
}