using UnityEngine;

[CreateAssetMenu(fileName = "NewPatient", menuName = "ArcaneWard/Patient")]
public class PatientData : ScriptableObject
{
    public string characterName;
    public RuntimeAnimatorController animator; 
    
    [Header("Visuals")]
    public Color sickColor = Color.white;      
    public Color curedColor = Color.green;     
    public Color altColor = Color.cyan;        
    
    [Header("Alternate Forms")]
    public Sprite idealSelfSprite; // Drag the Calico sprite here in the Inspector
}