using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip testClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = testClip;
        audioSource.Play();

        Debug.Log("Audio playing: " + audioSource.isPlaying);
        Debug.Log("Audio time: " + audioSource.time);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Forcing play - isPlaying: " + audioSource.isPlaying + ", time: " + audioSource.time);
            audioSource.Play();
        }
    }
}