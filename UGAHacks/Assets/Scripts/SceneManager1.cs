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
        IsPlaying = true;
        musicSource.clip = currentBeatMap.audioClip;
        musicSource.PlayDelayed(currentBeatMap.startDelay);
        SongStartDspTime = (float)AudioSettings.dspTime + currentBeatMap.startDelay;
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

        if (abs <= perfectWindow) { judgement = "PERFECT"; points = perfectScore; }
        else if (abs <= greatWindow) { judgement = "GREAT"; points = greatScore; }
        else if (abs <= goodWindow) { judgement = "GOOD"; points = goodScore; }
        else { RegisterMiss(); return; }

        Combo++;
        if (Combo > MaxCombo) MaxCombo = Combo;
        int earned = points * (1 + Combo / 10);
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

            // --- THIS IS THE NEW PART ---
            // Tell the Minigame Controller we are done
            MinigameController controller = FindObjectOfType<MinigameController>();
            if(controller != null) controller.WinGame();
        }
    }
}