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
        gameManager = ClickGameManager.Instance;

        // Pause the game until countdown finishes
        gameManager.SetPlaying(false);
        gameManager.clickButton.gameObject.SetActive(false);

        StartCoroutine(RunCountdown());
    }

    IEnumerator RunCountdown()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.fontSize = 120;

            // Quick scale animation
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1f, timer);
                countdownText.transform.localScale = Vector3.one * scale;
                yield return null;
            }
        }

        countdownText.text = "GO!";
        countdownText.color = Color.green;
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);
        countdownText.color = Color.white;

        // Now start the actual game
        gameManager.clickButton.gameObject.SetActive(true);
        gameManager.SetPlaying(true);

    }
}