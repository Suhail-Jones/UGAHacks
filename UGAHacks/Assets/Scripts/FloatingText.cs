using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public void Init(string text, Color color)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = color;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(0, 1, 0); 

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            Color c = GetComponent<TextMeshProUGUI>().color;
            c.a = Mathf.Lerp(1, 0, elapsed / duration);
            GetComponent<TextMeshProUGUI>().color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}