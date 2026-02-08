using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownIntro : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;

    private ClickGameManager gameManager;

    void Start()
    {
        // 1. Get the Manager
        gameManager = ClickGameManager.Instance;

        // 2. PAUSE THE GAME
        // Since we can't change the private variables inside ClickGameManager,
        // we simply disable the script. This stops 'Update()' from running,
        // so the timer won't go down.
        gameManager.enabled = false;

        // 3. Hide the button so the player can't click
        if (gameManager.clickButton != null)
            gameManager.clickButton.gameObject.SetActive(false);

        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            
            // Reset scale for punch effect
            countdownText.transform.localScale = Vector3.one * 1.5f;

            // Animate scale over 1 second
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1f, timer);
                countdownText.transform.localScale = Vector3.one * scale;
                yield return null;
            }
        }

        // "GO!" Sequence
        countdownText.text = "GO!";
        countdownText.color = Color.green;
        countdownText.transform.localScale = Vector3.one * 1.5f; // Big pop for GO
        
        yield return new WaitForSeconds(0.5f);

        // Cleanup Text
        countdownText.gameObject.SetActive(false);
        countdownText.color = Color.white;

        // 4. UNPAUSE THE GAME
        // Re-enable the script. The Update() loop will start running again,
        // and the timer will resume from where it started.
        gameManager.enabled = true;
        
        // Show the button again
        if (gameManager.clickButton != null)
            gameManager.clickButton.gameObject.SetActive(true);
    }
}