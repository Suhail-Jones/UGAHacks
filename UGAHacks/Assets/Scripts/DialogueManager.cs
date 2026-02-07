using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Setup")]
    public TextAsset inkJSONAsset; 
    private Story story;

    [Header("UI Connections")]
    public TextMeshProUGUI dialogueText;
    
    [Header("Fixed Choice Buttons")]
    // Drag your 2 existing scene buttons here
    public Button choice1Button;
    public Button choice2Button;
    private TextMeshProUGUI choice1Text;
    private TextMeshProUGUI choice2Text;

    [Header("Game Connections")]
    public SpriteRenderer patientRenderer;
    public Sprite sickSprite;
    public Sprite curedSprite;
    public Sprite frozenSprite;
    public ParticleSystem iceParticles;
    public ParticleSystem windParticles;

    void Awake()
    {
        // Cache the text components from the assigned buttons
        choice1Text = choice1Button.GetComponentInChildren<TextMeshProUGUI>();
        choice2Text = choice2Button.GetComponentInChildren<TextMeshProUGUI>();

        // Add listeners once at the start
        choice1Button.onClick.AddListener(() => OnChoiceSelected(0));
        choice2Button.onClick.AddListener(() => OnChoiceSelected(1));
    }

    void Start()
    {
        if (inkJSONAsset != null)
        {
            StartStory();
        }
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        
        // BIND FUNCTIONS
        story.BindExternalFunction("CastSpell", (string spell) => {
            if(spell == "ice" && iceParticles != null) iceParticles.Play();
            if(spell == "wind" && windParticles != null) windParticles.Play();
        });

        story.BindExternalFunction("ChangeSprite", (string state) => {
            if(state == "sick") patientRenderer.sprite = sickSprite;
            if(state == "cured") patientRenderer.sprite = curedSprite;
            if(state == "frozen") patientRenderer.sprite = frozenSprite;
        });

        RefreshUI();
    }

    void RefreshUI()
    {
        // 1. Load the text until the next choice point
        if (story.canContinue)
        {
            dialogueText.text = story.ContinueMaximally();
            HandleTags(story.currentTags);
        }

        // 2. Handle the 2 Fixed Buttons
        if (story.currentChoices.Count >= 2)
        {
            // Show buttons and set text
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(true);

            choice1Text.text = story.currentChoices[0].text;
            choice2Text.text = story.currentChoices[1].text;
        }
        else if (story.currentChoices.Count == 1)
        {
            // Only one choice available
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(false);
            choice1Text.text = story.currentChoices[0].text;
        }
        else
        {
            // No choices (End of story or middle of dialogue)
            choice1Button.gameObject.SetActive(false);
            choice2Button.gameObject.SetActive(false);
        }
    }

    void OnChoiceSelected(int index)
    {
        story.ChooseChoiceIndex(index);
        RefreshUI();
    }

    void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2) continue;

            string key = splitTag[0].Trim();
            string value = splitTag[1].Trim();

            if (key == "sprite")
            {
                if (value == "sick") patientRenderer.sprite = sickSprite;
                if (value == "cured") patientRenderer.sprite = curedSprite;
                if (value == "frozen") patientRenderer.sprite = frozenSprite;
            }
        }
    }
}