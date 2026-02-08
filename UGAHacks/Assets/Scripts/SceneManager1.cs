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
    
    // NEW: Tracks the exact second the last note should be hit
    private float lastNoteTime = 0f;

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
        
        if(currentBeatMap != null && currentBeatMap.audioClip != null)
        {
            // 1. CALCULATE END TIME based on the notes
            lastNoteTime = 0f;
            foreach (var note in currentBeatMap.notes)
            {
                if (note.hitTime > lastNoteTime) 
                    lastNoteTime = note.hitTime;
            }

            // 2. Start Audio
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
        if (!IsPlaying) return;

        float currentTime = GetCurrentSongTime();

        // CHECK END CONDITION:
        // If current time is past the last note time + a small buffer (1.5 seconds)
        // The buffer allows the last note to visually clear the screen or be registered as a Miss
        if (currentTime > lastNoteTime + 1.5f)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        if (!IsPlaying) return;
        
        IsPlaying = false;
        musicSource.Stop(); // Cut the music if it's still playing
        OnSongFinished?.Invoke();

        ReturnToStage();
    }

    void ReturnToStage()
    {
        // 1. Try to find the Minigame Controller
        MinigameController controller = FindObjectOfType<MinigameController>();
        
        if (controller != null)
        {
            controller.WinGame();
        }
        else
        {
            // 2. Fail-Safe: If MinigameController is missing, talk to GameManager directly
            Debug.LogWarning("MinigameController not found! Calling GameManager directly.");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndMinigameScene(true);
            }
        }
    }
}