using UnityEngine;
using TMPro;

public class ClickerGame : MonoBehaviour
{
    public RectTransform targetButton;
    public TextMeshProUGUI scoreText;
    public int requiredScore = 10;
    private int currentScore = 0;

    public void OnClick()
    {
        currentScore++;
        scoreText.text = currentScore + "/" + requiredScore;
        
        float x = Random.Range(-200, 200);
        float y = Random.Range(-100, 100);
        targetButton.anchoredPosition = new Vector2(x, y);

        if(currentScore >= requiredScore) FindObjectOfType<MinigameController>().WinGame();
    }
}