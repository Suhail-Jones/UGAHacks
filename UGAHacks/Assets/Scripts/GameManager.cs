using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("System Connections")]
    public DialogueManager dialogueManager;
    public StressManager stressManager;
    public GameObject gameOverScreen;
    public GameObject stageGroup;

    [Header("Scene Connections")]
    public SpriteRenderer patientRenderer;
    public Animator patientAnimator;
    public Transform spawnPoint;
    public Transform centerPoint;

    [Header("Patient Data")]
    public PatientData[] allPatients;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bellSound;
    public AudioClip magicSound;
    public AudioClip failSound;

    [Header("Background Music")]
    public AudioSource musicSource;
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private PatientData currentPatient;
    private string activeMinigameScene;
    private Coroutine walkCoroutine;
    private Dictionary<string, PatientData> patientLookup;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        patientLookup = new Dictionary<string, PatientData>();
        foreach (var p in allPatients)
        {
            patientLookup[p.characterName] = p;
        }

        StartMusic();
    }

    void StartMusic()
    {
        if (musicSource == null || bgmClip == null) return;

        musicSource.clip = bgmClip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.UnPause();
    }

    public void SpawnPatient(string name)
    {
        if (!patientLookup.TryGetValue(name, out currentPatient))
        {
            Debug.LogError("Unknown patient: " + name);
            return;
        }

        patientAnimator.enabled = true;
        patientAnimator.runtimeAnimatorController = currentPatient.animator;
        patientAnimator.Rebind();
        patientAnimator.Update(0f);

        patientRenderer.color = currentPatient.sickColor;
        patientRenderer.transform.localScale = currentPatient.scale;

        if (currentPatient.defaultSprite != null)
        {
            patientRenderer.sprite = currentPatient.defaultSprite;
        }

        if (walkCoroutine != null) StopCoroutine(walkCoroutine);

        patientRenderer.transform.position = spawnPoint.position;
        walkCoroutine = StartCoroutine(WalkToCenter());

        PlaySound("bell");
    }

    public void TransformPatient(string form)
    {
        if (currentPatient == null) return;

        if (form == "ideal" && currentPatient.idealSelfSprite != null)
        {
            patientAnimator.enabled = false;
            patientRenderer.sprite = currentPatient.idealSelfSprite;
            patientRenderer.color = currentPatient.altColor;
            PlaySound("magic");
        }
        else if (form == "normal")
        {
            patientAnimator.enabled = true;
            patientAnimator.runtimeAnimatorController = currentPatient.animator;
            patientAnimator.Rebind();
            patientAnimator.SetBool("isWalking", false);
            patientAnimator.Update(0f);

            patientRenderer.color = currentPatient.curedColor;
        }
    }

    public void LoadMinigameScene(string sceneName)
    {
        activeMinigameScene = sceneName;

        if (stageGroup != null) stageGroup.SetActive(false);

        PauseMusic();

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

public void EndMinigameScene(bool won)
    {
        SceneManager.UnloadSceneAsync(activeMinigameScene);

        if (stageGroup != null) stageGroup.SetActive(true);

        ResumeMusic();

        if (won) stressManager.ModifyStress(-20);
        else stressManager.ModifyStress(20);
        
        // CRITICAL CHANGE: We removed "dialogueManager.ContinueStory();"
        // The text ("Did it work?") is already waiting in the text box because Ink 
        // loaded it while starting the game. We just needed to unhide the stage to see it.
        // The player will click "Continue" manually to read the next line.
        
        // Ensure the Dialogue UI is visible (in case it was hidden)
        dialogueManager.gameObject.SetActive(true);
    }

    public void TriggerGameOver()
    {
        PauseMusic();
        if (gameOverScreen) gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void PlaySound(string soundName)
    {
        if (!audioSource) return;
        if (soundName == "bell") audioSource.PlayOneShot(bellSound);
        if (soundName == "magic") audioSource.PlayOneShot(magicSound);
        if (soundName == "fail") audioSource.PlayOneShot(failSound);
    }

    IEnumerator WalkToCenter()
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 start = spawnPoint.position;
        Vector3 end = centerPoint.position;

        patientAnimator.SetBool("isWalking", true);

        while (elapsed < duration)
        {
            patientRenderer.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        patientRenderer.transform.position = end;

        patientAnimator.SetBool("isWalking", false);
    }
}