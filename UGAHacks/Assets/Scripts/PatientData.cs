using UnityEngine;

[CreateAssetMenu(fileName = "NewPatient", menuName = "ArcaneWard/Patient")]
public class PatientData : ScriptableObject
{
    public string characterName;
    public RuntimeAnimatorController animator;

    [Header("Visuals")]
    public Sprite defaultSprite;
    public Color sickColor = Color.white;
    public Color curedColor = Color.green;
    public Color altColor = Color.cyan;
    public Vector3 scale = Vector3.one;    // NEW: per-patient size

    [Header("Alternate Forms")]
    public Sprite idealSelfSprite;
}