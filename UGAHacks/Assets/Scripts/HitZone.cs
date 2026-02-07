using UnityEngine;
using System.Collections.Generic;

public class HitZone : MonoBehaviour
{
    public NoteData.Lane lane;
    public KeyCode inputKey;

    [Header("Visual Feedback")]
    public SpriteRenderer glowRenderer; // optional glow child

    private List<Note> pendingNotes = new List<Note>();
    private Color originalColor;

    void Start()
    {
        if (glowRenderer != null)
            originalColor = glowRenderer.color;

        // Auto-assign keys if not set
        if (inputKey == KeyCode.None)
        {
            switch (lane)
            {
                case NoteData.Lane.Left: inputKey = KeyCode.LeftArrow; break;
                case NoteData.Lane.Down: inputKey = KeyCode.DownArrow; break;
                case NoteData.Lane.Up: inputKey = KeyCode.UpArrow; break;
                case NoteData.Lane.Right: inputKey = KeyCode.RightArrow; break;
            }
        }
    }

    public void RegisterNote(Note note)
    {
        pendingNotes.Add(note);
    }

    void Update()
    {
        // Clean up inactive notes
        pendingNotes.RemoveAll(n => n == null || !n.IsActive);

        if (Input.GetKeyDown(inputKey))
        {
            PressEffect();
            TryHitNote();
        }
    }

    void TryHitNote()
    {
        if (pendingNotes.Count == 0)
            return;

        float songTime = SceneManager.Instance.GetCurrentSongTime();

        // Find the closest note within the miss window
        Note closest = null;
        float closestDiff = float.MaxValue;

        foreach (Note note in pendingNotes)
        {
            if (note == null || !note.IsActive) continue;

            float diff = songTime - note.Data.hitTime;
            float abs = Mathf.Abs(diff);

            if (abs < closestDiff && abs <= SceneManager.Instance.missWindow)
            {
                closest = note;
                closestDiff = abs;
            }
        }

        if (closest != null)
        {
            float timeDiff = songTime - closest.Data.hitTime;
            SceneManager.Instance.RegisterHit(timeDiff);
            closest.Hit();
            pendingNotes.Remove(closest);
        }
    }

    void PressEffect()
    {
        if (glowRenderer != null)
        {
            glowRenderer.color = Color.white;
            Invoke(nameof(ResetGlow), 0.1f);
        }
    }

    void ResetGlow()
    {
        if (glowRenderer != null)
            glowRenderer.color = originalColor;
    }
}