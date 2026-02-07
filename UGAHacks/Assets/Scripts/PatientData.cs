using UnityEngine;

[CreateAssetMenu(fileName = "NewPatient", menuName = "ArcaneWard/Patient")]
public class PatientData : ScriptableObject
{
    public string characterName;
    
    // The Animation Controller (e.g., Anim_Skeleton)
    public RuntimeAnimatorController animator; 
    
    [Header("Visuals")]
    public Color sickColor = Color.white;      // Normal (White)
    public Color curedColor = Color.green;     // Healthy (Green)
    public Color frozenColor = Color.cyan;     // Frozen (Blue)
}