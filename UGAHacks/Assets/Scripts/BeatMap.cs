using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBeatMap", menuName = "Rhythm Game/Beat Map")]
public class BeatMap : ScriptableObject
{
    public string songName;
    public AudioClip audioClip;
    public float bpm = 120f;

    [Tooltip("Seconds before the first note to let the player get ready")]
    public float startDelay = 3f;

    [Tooltip("Time (in seconds from song start) each note should be HIT")]
    public List<NoteData> notes = new List<NoteData>();
}

[Serializable]
public class NoteData
{
    public enum Lane { Left, Down, Up, Right }
    public Lane lane;

    [Tooltip("Time in seconds (from song start) when this note should be perfectly hit")]
    public float hitTime;
}