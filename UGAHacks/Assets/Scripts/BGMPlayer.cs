using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float volume = 0.5f;

    void Start()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = bgmClip;
        source.volume = volume;
        source.loop = true;
        source.Play();
    }
}
