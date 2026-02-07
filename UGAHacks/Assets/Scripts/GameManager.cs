using UnityEngine;
using System.Collections; // Required for Coroutines

public class GameManager : MonoBehaviour
{
    [Header("Scene Connections")]
    // Drag the 'PatientSprite' object from your scene here
    public SpriteRenderer patientRenderer; 
    public Animator patientAnimator;
    
    // Drag your empty GameObjects for positions
    public Transform spawnPoint;  
    public Transform centerPoint;

    [Header("Patient Data")]
    // Drag your 'SkeletonData' and 'SwampyData' ScriptableObjects here
    public PatientData skeletonData;
    public PatientData swampyData;

    [Header("Audio")]
    public AudioSource audioSource; // Add an AudioSource component to GameManager and drag it here
    public AudioClip bellSound;
    public AudioClip magicSound;
    public AudioClip freezeSound;

    private PatientData currentPatient;

    // --- FUNCTIONS CALLED BY DIALOGUE MANAGER ---

    public void SpawnPatient(string name)
    {
        // 1. Find the correct data
        if (name == "skeleton") currentPatient = skeletonData;
        else if (name == "swampy") currentPatient = swampyData;
        else 
        {
            Debug.LogError("Unknown patient: " + name);
            return;
        }

        // 2. Apply Visuals
        if (currentPatient.animator != null)
            patientAnimator.runtimeAnimatorController = currentPatient.animator;
        
        patientRenderer.color = currentPatient.sickColor;

        // 3. Teleport to Spawn Point and Walk In
        patientRenderer.transform.position = spawnPoint.position;
        StartCoroutine(WalkToCenter());
    }

    public void ChangePatientState(string state)
    {
        if (currentPatient == null) return;

        if (state == "cured")
        {
            patientRenderer.color = currentPatient.curedColor;
            PlaySound("magic");
        }
        else if (state == "frozen")
        {
            patientRenderer.color = currentPatient.frozenColor;
            PlaySound("freeze");
        }
    }

    public void PlaySound(string soundName)
    {
        if (audioSource == null) return;

        if (soundName == "bell") audioSource.PlayOneShot(bellSound);
        if (soundName == "magic") audioSource.PlayOneShot(magicSound);
        if (soundName == "freeze") audioSource.PlayOneShot(freezeSound);
    }

    // --- HELPER MOVEMENTS ---

    IEnumerator WalkToCenter()
    {
        // Simple movement logic (no external plugins needed)
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