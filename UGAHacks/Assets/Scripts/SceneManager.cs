using UnityEngine;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

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

    // State
    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int MaxCombo { get; private set; }
    public bool IsPlaying { get; private set; }

    public float SongTime => musicSource.time;
    public float SongStartDspTime { get; private set; }

    // Events for UI
    public event Action<string, int> OnNoteJudged;   // (judgement, combo)
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
        IsPlaying = true;

        Debug.Log("Music Source: " + musicSource);
        Debug.Log("Audio Clip: " + currentBeatMap.audioClip);
        Debug.Log("Start Delay: " + currentBeatMap.startDelay);

        musicSource.clip = currentBeatMap.audioClip;

        Debug.Log("Clip assigned to source: " + musicSource.clip);
        Debug.Log("About to play with delay: " + currentBeatMap.startDelay);

        musicSource.PlayDelayed(currentBeatMap.startDelay);
        SongStartDspTime = (float)AudioSettings.dspTime + currentBeatMap.startDelay;

        Debug.Log("PlayDelayed called. IsPlaying: " + musicSource.isPlaying);
    }

    /// <summary>
    /// Returns the current song position in seconds, accounting for the start delay.
    /// </summary>
    public float GetCurrentSongTime()
    {
        return (float)AudioSettings.dspTime - SongStartDspTime;
    }

    public void RegisterHit(float timeDifference)
    {
        float abs = Mathf.Abs(timeDifference);
        string judgement;
        int points;

        if (abs <= perfectWindow)
        {
            judgement = "PERFECT";
            points = perfectScore;
        }
        else if (abs <= greatWindow)
        {
            judgement = "GREAT";
            points = greatScore;
        }
        else if (abs <= goodWindow)
        {
            judgement = "GOOD";
            points = goodScore;
        }
        else
        {
            // Within missWindow but not good — treat as miss
            RegisterMiss();
            return;
        }

        Combo++;
        if (Combo > MaxCombo) MaxCombo = Combo;

        int earned = points * (1 + Combo / 10); // small combo multiplier
        Score += earned;

        OnNoteJudged?.Invoke(judgement, Combo);
        OnScoreChanged?.Invoke(Score);
    }

    public void RegisterMiss()
    {
        Combo = 0;
        OnNoteJudged?.Invoke("MISS", 0);
    }

    void Update()
    {
        if (IsPlaying && !musicSource.isPlaying && GetCurrentSongTime() > 1f)
        {
            IsPlaying = false;
            OnSongFinished?.Invoke();
        }
    }
}