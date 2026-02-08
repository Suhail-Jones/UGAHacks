using UnityEngine;
using System;

public class SceneManager1 : MonoBehaviour
{
    public static SceneManager1 Instance { get; private set; }

    [Header("References")]
    public BeatMap currentBeatMap;
    public AudioSource musicSource;

    [Header("Timing Windows (seconds)")]
    public float perfectWindow = 0.05f;
    public float greatWindow = 0.10f;
    public float goodWindow = 0.15f;
    public float missWindow = 0.25f;

    [Header("Score Values")]
    public int perfectScore = 300;
    public int greatScore = 200;
    public int goodScore = 100;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public bool IsPlaying { get; private set; }

    public float SongTime => musicSource.time;
    public float SongStartDspTime { get; private set; }

    private float lastNoteTime = 0f;

    // Performance tracking
    private float performanceTotal = 0f;
    private int notesProcessed = 0;

    public event Action<string, int> OnNoteJudged;
    public event Action<int> OnScoreChanged;
    public event Action OnSongFinished;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartSong();
    }

    public void StartSong()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        performanceTotal = 0f;
        notesProcessed = 0;
        IsPlaying = true;

        if (currentBeatMap != null && currentBeatMap.audioClip != null)
        {
            lastNoteTime = 0f;
            foreach (var note in currentBeatMap.notes)
            {
                if (note.hitTime > lastNoteTime)
                    lastNoteTime = note.hitTime;
            }

            musicSource.clip = currentBeatMap.audioClip;
            musicSource.PlayDelayed(currentBeatMap.startDelay);
            SongStartDspTime = (float)AudioSettings.dspTime + currentBeatMap.startDelay;
        }
        else
        {
            Debug.LogError("BeatMap or Audio Clip is missing!");
        }
    }

    public float GetCurrentSongTime()
    {
        return (float)AudioSettings.dspTime - SongStartDspTime;
    }

    public void RegisterHit(float timeDifference)
    {
        float abs = Mathf.Abs(timeDifference);
        string judgement;
        int points;
        float notePerformance;

        if (abs <= perfectWindow)
        {
            judgement = "PERFECT"; points = perfectScore; notePerformance = 1.0f;
        }
        else if (abs <= greatWindow)
        {
            judgement = "GREAT"; points = greatScore; notePerformance = 0.75f;
        }
        else if (abs <= goodWindow)
        {
            judgement = "GOOD"; points = goodScore; notePerformance = 0.5f;
        }
        else
        {
            RegisterMiss();
            return;
        }

        // Track performance for this note
        performanceTotal += notePerformance;
        notesProcessed++;

        Combo++;
        if (Combo > MaxCombo) MaxCombo = Combo;
        int earned = points * (1 + Combo / 10);
        Score += earned;

        OnNoteJudged?.Invoke(judgement, Combo);
        OnScoreChanged?.Invoke(Score);
    }

    public void RegisterMiss()
    {
        // Miss = 0 performance for this note
        notesProcessed++;

        Combo = 0;
        OnNoteJudged?.Invoke("MISS", 0);
    }

    /// <summary>
    /// Returns 0.0 (all misses) to 1.0 (all perfects).
    /// </summary>
    public float GetPerformance()
    {
        if (notesProcessed <= 0) return 0f;
        return Mathf.Clamp01(performanceTotal / notesProcessed);
    }

    void Update()
    {
        if (!IsPlaying) return;

        float currentTime = GetCurrentSongTime();

        if (currentTime > lastNoteTime + 1.5f)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        musicSource.Stop();
        OnSongFinished?.Invoke();

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