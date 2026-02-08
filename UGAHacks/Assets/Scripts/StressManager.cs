using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StressManager : MonoBehaviour
{
    public Slider stressSlider;
    public Image fillImage;
    public Transform textSpawnPoint;
    public GameObject floatingTextPrefab; 
    public GameManager gameManager;

    public float currentStress = 0f;
    public float maxStress = 100f;
    public int reputation = 50;

    void Start() { UpdateUI(); }

    public void ModifyStress(float amount)
    {
        currentStress += amount;
        if (currentStress < 0) currentStress = 0;
        
        if (amount > 0) SpawnText($"+{amount}", Color.red);
        else SpawnText($"{amount}", Color.green);

        if (currentStress >= maxStress)
        {
            currentStress = maxStress;
            gameManager.TriggerGameOver();
        }
        UpdateUI();
    }

    void SpawnText(string msg, Color col)
    {
        if(floatingTextPrefab) {
            GameObject go = Instantiate(floatingTextPrefab, textSpawnPoint);
            go.GetComponent<FloatingText>().Init(msg, col);
        }
    }

    void UpdateUI()
    {
        if(stressSlider) stressSlider.value = currentStress / maxStress;
        if(fillImage) fillImage.color = Color.Lerp(Color.green, Color.red, currentStress / maxStress);
    }

    public void FinalizeReputation()
    {
        if (currentStress < 30) reputation += 10;
        else if (currentStress < 70) reputation += 5;
        else reputation -= 15;
        currentStress = 0; 
        UpdateUI();
    }
}