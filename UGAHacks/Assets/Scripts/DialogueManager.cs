using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSON;
    public GameManager gameManager;
    public TextMeshProUGUI storyText; 
    
    [Header("Choice Buttons")]
    public Button choiceButton1; 
    public Button choiceButton2; 
    public Button choiceButton3; // NEW 3rd Button
    public Button continueButton; 

    private Story story;

    void Start() { StartStory(); }

    void StartStory()
    {
        story = new Story(inkJSON.text);

        // Bindings matching Story.ink
        story.BindExternalFunction("Spawn", (string name) => gameManager.SpawnPatient(name));
        story.BindExternalFunction("Transform", (string form) => gameManager.TransformPatient(form));
        story.BindExternalFunction("LoadMinigame", (string name) => gameManager.LoadMinigameScene(name));
        story.BindExternalFunction("Stress", (float val) => gameManager.stressManager.ModifyStress(val));
        story.BindExternalFunction("EndPatient", () => gameManager.OnEndPatient());

        continueButton.onClick.AddListener(()=>RefreshUI());
        RefreshUI();
    }
    
    public void ContinueStory() { RefreshUI(); }

    void Update()
    {
        // Add Keyboard support for the 3rd choice
        if (story.currentChoices.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) 
                MakeChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) 
                MakeChoice(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) 
                MakeChoice(2);
        }
    }

    void MakeChoice(int index)
    {
        if (index < story.currentChoices.Count)
        {
            story.ChooseChoiceIndex(index);
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        // 1. Hide Everything
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        // 2. Show Text if available
        if (story.canContinue)
        {
            storyText.gameObject.SetActive(true);
            storyText.text = story.Continue();
            continueButton.gameObject.SetActive(true);
        }
        // 3. Show Choices if available
        else if (story.currentChoices.Count > 0)
        {
            storyText.gameObject.SetActive(false); // Hide story text to focus on choices
            
            if (story.currentChoices.Count >= 1) SetupBtn(choiceButton1, 0, story.currentChoices[0]);
            if (story.currentChoices.Count >= 2) SetupBtn(choiceButton2, 1, story.currentChoices[1]);
            if (story.currentChoices.Count >= 3) SetupBtn(choiceButton3, 2, story.currentChoices[2]);
        }
    }

    void SetupBtn(Button btn, int index, Choice choice)
    {
        btn.gameObject.SetActive(true);
        
        // FORMATTING: This creates the "[1] Response" style you requested
        string formattedText = "[" + (index + 1) + "] " + choice.text;
        btn.GetComponentInChildren<TextMeshProUGUI>().text = formattedText;
        
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            MakeChoice(index);
        });
    }
}