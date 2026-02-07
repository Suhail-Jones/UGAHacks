using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefab & Lanes")]
    public GameObject notePrefab;

    [Tooltip("Assign the 4 lane targets (hit zones) in order: Left, Down, Up, Right")]
    public Transform[] laneTargets = new Transform[4];

    [Tooltip("Where notes spawn (Y position above the hit zone)")]
    public float spawnYOffset = 8f;

    [Tooltip("How many seconds before the hit-time to spawn the note (= travel time)")]
    public float travelTime = 2f;

    // Internal
    private BeatMap beatMap;
    private int nextNoteIndex;
    private bool finished;

    // Sprites per lane (set via inspector or loaded)
    public Sprite[] laneSprites = new Sprite[4]; // Left, Down, Up, Right

    void Start()
    {
        beatMap = SceneManager1.Instance.currentBeatMap;
        nextNoteIndex = 0;
        finished = false;
    }

    void Update()
    {
        if (finished || beatMap == null) return;

        float songTime = SceneManager1.Instance.GetCurrentSongTime();

        // Spawn notes that need to appear now (travelTime seconds before their hitTime)
        while (nextNoteIndex < beatMap.notes.Count)
        {
            NoteData data = beatMap.notes[nextNoteIndex];
            float spawnTime = data.hitTime - travelTime;

            if (songTime >= spawnTime)
            {
                SpawnNote(data);
                nextNoteIndex++;
            }
            else
            {
                break; // notes are sorted by hitTime
            }
        }

        if (nextNoteIndex >= beatMap.notes.Count)
            finished = true;
    }

    void SpawnNote(NoteData data)
    {
        int laneIndex = (int)data.lane;
        Transform target = laneTargets[laneIndex];

        Vector3 spawnPos = new Vector3(target.position.x, target.position.y + spawnYOffset, 0f);
        GameObject noteGO = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        Note note = noteGO.GetComponent<Note>();
        note.Initialize(data, target.position, travelTime);

        // Set sprite/color
        SpriteRenderer sr = noteGO.GetComponent<SpriteRenderer>();
        if (laneSprites != null && laneIndex < laneSprites.Length && laneSprites[laneIndex] != null)
        {
            sr.sprite = laneSprites[laneIndex];
        }
        else
        {
            // Fallback: tint by lane
            Color[] colors = { Color.magenta, Color.cyan, Color.green, Color.red };
            sr.color = colors[laneIndex];
        }

        // Register with the correct hit zone
        HitZone zone = target.GetComponent<HitZone>();
        if (zone != null) zone.RegisterNote(note);
    }
}