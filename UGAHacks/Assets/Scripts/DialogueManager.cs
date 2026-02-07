using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    public TextAsset inkJSON;
    private Story story;

    [Header("External Connections")]
    public GameManager gameManager;

    [Header("UI Components")]
    public TextMeshProUGUI storyText; 
    
    // THE NEW WAY: Drag your 2 actual buttons here
    public Button choiceButton1; 
    public Button choiceButton2;

    // THE INVISIBLE BUTTON: Drag a button that covers the screen here
    // (This is needed to click through normal text lines)
    public Button continueButton; 

    void Start()
    {
        StartStory();
    }

    void StartStory()
    {
        story = new Story(inkJSON.text);

        // --- BIND FUNCTIONS ---
        story.BindExternalFunction("Spawn", (string name) => gameManager.SpawnPatient(name));
        story.BindExternalFunction("ChangeState", (string state) => gameManager.ChangePatientState(state));
        story.BindExternalFunction("PlaySound", (string soundName) => gameManager.PlaySound(soundName));

        // Setup the Continue Button
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => RefreshUI());

        RefreshUI();
    }

    void RefreshUI()
    {
        // 1. RESET BUTTONS (Turn them off and clear old clicks)
        choiceButton1.gameObject.SetActive(false);
        choiceButton1.onClick.RemoveAllListeners();
        
        choiceButton2.gameObject.SetActive(false);
        choiceButton2.onClick.RemoveAllListeners();

        // 2. READ NEXT LINE (If available)
        if (story.canContinue)
        {
            string text = story.Continue();
            storyText.text = text;
            
            // Check for tags
            foreach(var tag in story.currentTags)
            {
                if(tag.Contains("spawn:skeleton")) gameManager.SpawnPatient("skeleton");
                if(tag.Contains("spawn:swampy")) gameManager.SpawnPatient("swampy");
            }
        }

        // 3. SHOW CHOICES OR SHOW CONTINUE BUTTON
        if (story.currentChoices.Count > 0)
        {
            // We have choices: Hide the "Next" button so they MUST pick an option
            continueButton.gameObject.SetActive(false);

            // --- SETUP BUTTON 1 ---
            if (story.currentChoices.Count >= 1)
            {
                var choice = story.currentChoices[0];
                choiceButton1.gameObject.SetActive(true); // Turn it on
                choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                
                choiceButton1.onClick.AddListener(() => {
                    story.ChooseChoiceIndex(choice.index);
                    RefreshUI();
                });
            }

            // --- SETUP BUTTON 2 ---
            if (story.currentChoices.Count >= 2)
            {
                var choice = story.currentChoices[1];
                choiceButton2.gameObject.SetActive(true); // Turn it on
                choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                
                choiceButton2.onClick.AddListener(() => {
                    story.ChooseChoiceIndex(choice.index);
                    RefreshUI();
                });
            }
        }
        else
        {
            // No choices? Show the invisible "Next" button so clicking anywhere advances text
            continueButton.gameObject.SetActive(true);
        }
    }
}