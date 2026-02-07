using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI judgementText;
    public TextMeshProUGUI songInfoText;

    [Header("Results Panel")]
    public GameObject resultsPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI maxComboText;

    private Coroutine judgementFade;

    void Start()
    {
        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        SceneManager1 gm = SceneManager1.Instance;
        gm.OnScoreChanged += UpdateScore;
        gm.OnNoteJudged += ShowJudgement;
        gm.OnSongFinished += ShowResults;

        UpdateScore(0);
        comboText.text = "";
        judgementText.text = "";

        if (songInfoText != null)
            songInfoText.text = gm.currentBeatMap.songName;
    }

    void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score:N0}";
    }

    void ShowJudgement(string judgement, int combo)
    {
        // Set colour based on judgement
        Color col = judgement switch
        {
            "PERFECT" => Color.yellow,
            "GREAT" => Color.green,
            "GOOD" => new Color(0.5f, 0.8f, 1f),
            _ => Color.red
        };

        judgementText.text = judgement;
        judgementText.color = col;

        if (combo > 1)
            comboText.text = $"{combo}x COMBO";
        else if (combo == 0)
            comboText.text = "";

        if (judgementFade != null) StopCoroutine(judgementFade);
        judgementFade = StartCoroutine(FadeText(judgementText, 0.8f));
    }

    IEnumerator FadeText(TextMeshProUGUI tmp, float duration)
    {
        float timer = 0f;
        Color start = tmp.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            tmp.color = new Color(start.r, start.g, start.b, alpha);
            yield return null;
        }
        tmp.text = "";
    }

    void ShowResults()
    {
        if (resultsPanel == null) return;
        resultsPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {SceneManager1.Instance.Score:N0}";
        maxComboText.text = $"Max Combo: {SceneManager1.Instance.MaxCombo}x";
    }

    void OnDestroy()
    {
        if (SceneManager1.Instance == null) return;
        SceneManager1.Instance.OnScoreChanged -= UpdateScore;
        SceneManager1.Instance.OnNoteJudged -= ShowJudgement;
        SceneManager1.Instance.OnSongFinished -= ShowResults;
    }
}