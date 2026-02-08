using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("System Connections")]
    public DialogueManager dialogueManager;
    public StressManager stressManager;
    public GameObject gameOverScreen;
    
    // NEW: The object that holds all the Stage visuals
    public GameObject stageGroup; 

    [Header("Scene Connections")]
    public SpriteRenderer patientRenderer; 
    public Animator patientAnimator;
    public Transform spawnPoint;  
    public Transform centerPoint;

    [Header("Patient Data")]
    public PatientData witchKittyData; 
    public PatientData hoodedGuyData; 

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip bellSound;
    public AudioClip magicSound;
    public AudioClip failSound;

    private PatientData currentPatient;
    private string activeMinigameScene;

    void Awake() { if (Instance == null) Instance = this; }

    // --- CALLED BY INK ---
    public void SpawnPatient(string name)
    {
        if (name == "witchKitty") currentPatient = witchKittyData;
        else if (name == "hoodedGuy") currentPatient = hoodedGuyData;
        else { Debug.LogError("Unknown patient: " + name); return; }

        // Reset to normal form
        patientAnimator.runtimeAnimatorController = currentPatient.animator;
        patientRenderer.color = currentPatient.sickColor;
        
        // Teleport and Walk in
        patientRenderer.transform.position = spawnPoint.position;
        StartCoroutine(WalkToCenter());
        
        PlaySound("bell");
    }

    public void TransformPatient(string form)
    {
        if (currentPatient == witchKittyData && form == "ideal")
        {
            patientRenderer.sprite = currentPatient.idealSelfSprite;
            patientRenderer.color = currentPatient.altColor;
            PlaySound("magic");
        }
        else if (form == "normal")
        {
            patientAnimator.runtimeAnimatorController = currentPatient.animator;
            patientRenderer.color = currentPatient.sickColor;
        }
    }

    public void LoadMinigameScene(string sceneName)
    {
        activeMinigameScene = sceneName;
        
        // NEW: Hide the entire stage (Visuals, Canvas, Camera)
        if(stageGroup != null) stageGroup.SetActive(false);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); 
    }

    public void EndMinigameScene(bool won)
    {
        SceneManager.UnloadSceneAsync(activeMinigameScene);
        
        // NEW: Show the stage again
        if(stageGroup != null) stageGroup.SetActive(true);
        
        if (won) stressManager.ModifyStress(-20);
        else stressManager.ModifyStress(20);
        
        dialogueManager.ContinueStory();
    }

    public void TriggerGameOver()
    {
        if(gameOverScreen) gameOverScreen.SetActive(true);
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

        while (elapsed < duration)
        {
            patientRenderer.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        patientRenderer.transform.position = end;
    }
}