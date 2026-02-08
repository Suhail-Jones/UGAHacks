using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("System Connections")]
    public DialogueManager dialogueManager;
    public StressManager stressManager;
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

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public Button restartButton;
    public string startMenuSceneName = "StartScene";

    [Header("Minigame Stress")]
    [Tooltip("Stress added at 0% performance (worst)")]
    public float worstStressChange = 25f;
    [Tooltip("Stress added at 100% performance (best, should be negative)")]
    public float bestStressChange = -25f;

    private PatientData currentPatient;
    private string activeMinigameScene;
    private Coroutine walkCoroutine;
    private Dictionary<string, PatientData> patientLookup;
    private bool isGameOver = false;
    private bool isTransformed = false;

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

        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        StartMusic();
    }

    // ===================== MUSIC =====================

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

    // ===================== PATIENTS =====================

    public void SpawnPatient(string name)
    {
        if (!patientLookup.TryGetValue(name, out currentPatient))
        {
            Debug.LogError("Unknown patient: " + name);
            return;
        }

        isTransformed = false;

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
            isTransformed = true;
            PlaySound("magic");
        }
        else if (form == "normal")
        {
            patientAnimator.enabled = true;
            patientAnimator.runtimeAnimatorController = currentPatient.animator;
            patientAnimator.Rebind();
            patientAnimator.SetBool("isWalking", false);
            patientAnimator.Update(0f);

            if (currentPatient.defaultSprite != null)
                patientRenderer.sprite = currentPatient.defaultSprite;

            patientRenderer.color = currentPatient.curedColor;
            isTransformed = false;
        }
    }

    public void OnEndPatient()
    {
        isTransformed = false;
    }

    // ===================== MINIGAMES =====================

    public void LoadMinigameScene(string sceneName)
    {
        activeMinigameScene = sceneName;

        PauseMusic();

        if (stageGroup != null) stageGroup.SetActive(false);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void EndMinigameScene(float performance)
    {
        SceneManager.UnloadSceneAsync(activeMinigameScene);

        if (stageGroup != null) stageGroup.SetActive(true);

        ResetPatientToIdle();

        ResumeMusic();

        float stressChange = Mathf.Lerp(worstStressChange, bestStressChange, performance);
        stressManager.ModifyStress(stressChange);

        if (isGameOver) return;

        dialogueManager.gameObject.SetActive(true);
    }

     public void WinGame()
    {
        PauseMusic();
        SceneManager.LoadScene("WinScene");
    }
    // ===================== GAME OVER =====================

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        PauseMusic();
        PlaySound("fail");

        if (gameOverScreen != null) gameOverScreen.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene(startMenuSceneName);
    }

    // ===================== AUDIO =====================

    public void PlaySound(string soundName)
    {
        if (!audioSource) return;
        if (soundName == "bell") audioSource.PlayOneShot(bellSound);
        if (soundName == "magic") audioSource.PlayOneShot(magicSound);
        if (soundName == "fail") audioSource.PlayOneShot(failSound);
    }

    // ===================== WALK =====================

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

    // ===================== ANIMATOR RESET =====================

    void ResetPatientToIdle()
    {
        if (patientAnimator == null || currentPatient == null) return;

        // Don't override an active transform (e.g. calico form)
        if (isTransformed) return;

        patientAnimator.enabled = true;
        patientAnimator.Rebind();
        patientAnimator.SetBool("isWalking", false);
        patientAnimator.Update(0f);
    }
}