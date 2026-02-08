using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance { get; private set; }

    private GameObject panel;

    void Awake()
    {
        Instance = this;
    }

    public void Show(int finalStress)
    {
        StartCoroutine(ShowSequence(finalStress));
    }

    IEnumerator ShowSequence(int finalStress)
    {
        // --- Canvas ---
        GameObject canvasObj = new GameObject("WinScreenCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // --- Background Panel ---
        panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0);

        CanvasGroup cg = panel.AddComponent<CanvasGroup>();
        cg.alpha = 0;

        // --- Title ---
        CreateText("SHIFT COMPLETE", new Vector2(0, 160), 64,
                   new Color(1f, 0.85f, 0.3f));

        // --- Star Rating ---
        CreateText(GetRating(finalStress), new Vector2(0, 80), 40,
                   Color.white);

        // --- Stress Display ---
        CreateText($"Final Stress: {finalStress} / 50", new Vector2(0, 20), 32,
                   GetStressColor(finalStress));

        // --- Flavor Text ---
        CreateText(GetFlavorText(finalStress), new Vector2(0, -60), 24,
                   new Color(0.75f, 0.75f, 0.85f));

        // --- Play Again Button ---
        CreateButton("Play Again", new Vector2(-140, -180),
                     () => SceneManager.LoadScene(SceneManager.GetActiveScene().name));

        // --- Quit Button ---
        CreateButton("Quit", new Vector2(140, -180),
                     () => Application.Quit());

        // --- Fade In ---
        float t = 0f;
        float fadeDuration = 2f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = t / fadeDuration;
            cg.alpha = a;
            bg.color = new Color(0.03f, 0.01f, 0.08f, a * 0.95f);
            yield return null;
        }
        cg.alpha = 1f;
        bg.color = new Color(0.03f, 0.01f, 0.08f, 0.95f);
    }

    TextMeshProUGUI CreateText(string content, Vector2 pos, int size, Color color)
    {
        GameObject obj = new GameObject("WinText");
        obj.transform.SetParent(panel.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(900, 80);

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;

        return tmp;
    }

    void CreateButton(string label, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject("Button_" + label);
        btnObj.transform.SetParent(panel.transform, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(240, 60);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.15f, 0.4f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.25f, 0.15f, 0.4f);
        colors.highlightedColor = new Color(0.45f, 0.3f, 0.65f);
        colors.pressedColor = new Color(0.15f, 0.08f, 0.25f);
        btn.colors = colors;

        btn.onClick.AddListener(action);

        // Button label
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    string GetRating(int stress)
    {
        if (stress <= 5)  return "★★★ PERFECT DOCTOR ★★★";
        if (stress <= 15) return "★★★ EXCELLENT ★★★";
        if (stress <= 30) return "★★ GOOD WORK ★★";
        if (stress <= 40) return "★ BARELY MADE IT ★";
        return "★ SURVIVED ★";
    }

    Color GetStressColor(int stress)
    {
        if (stress <= 15) return new Color(0.3f, 1f, 0.4f);
        if (stress <= 30) return new Color(1f, 1f, 0.3f);
        return new Color(1f, 0.4f, 0.3f);
    }

    string GetFlavorText(int stress)
    {
        if (stress <= 5)
            return "You have a gift. Every word was exactly what they needed.";
        if (stress <= 15)
            return "The ward feels peaceful. Your patients left with real smiles.";
        if (stress <= 30)
            return "Not bad, Doc. A few rough moments, but you pulled through.";
        if (stress <= 40)
            return "That was close. The stress almost broke you both.";
        return "You survived the shift... but just barely. Take a breath.";
    }
}