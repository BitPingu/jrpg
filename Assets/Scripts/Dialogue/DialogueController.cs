using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } // Singleton instance

    public Dialogue dialogue;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage, continueImage;

    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;

    private int _dialogueIndex = 0;
    public bool IsDialogueActive { get; set; }
    private bool _isTyping;
    public bool DelaySkip { get; set; }
    public bool IsDialogueFinished { get; set; }

    [SerializeField] private PlayerController _input;

    public Dictionary<string, CharacterBase> CharsInDialogue = new Dictionary<string, CharacterBase>();
    private CharacterBase _currentChar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show); // Toggle UI visability
    }

    public void SetNPC()
    {
        _currentChar = CharsInDialogue[dialogue.Lines[_dialogueIndex].charName];
        SetNPCInfo(_currentChar.charName, _currentChar.portrait);
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    private void Update()
    {
        if (_input.E && IsDialogueActive && !DelaySkip)
        {
            // next line
            NextLine();
        }
    }

    public void StartDialogue()
    {
        IsDialogueActive = true;
        _dialogueIndex = 0;

        // set npc
        SetNPC();
        ShowDialogueUI(true);
        
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (_isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines(); // halt auto progress
            SetDialogueText(dialogue.Lines[_dialogueIndex].line);
            _isTyping = false;
            continueImage.enabled = true;
            StopVoice();
        }
        else if (++_dialogueIndex < dialogue.Lines.Length)
        {
            // set npc
            SetNPC();
            // if another line, type next line
            StartCoroutine(TypeLine());
        }
        else
        {
            // end dialogue
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        continueImage.enabled = false;
        _isTyping = true;
        SetDialogueText("");

        foreach(char letter in dialogue.Lines[_dialogueIndex].line)
        {
            SetDialogueText(dialogueText.text += letter);
            SFXManager.PlayVoice(_currentChar.voiceSound, _currentChar.voicePitch);
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
        continueImage.enabled = true;
        StopVoice();

        // auto progress
        if (dialogue.Lines[_dialogueIndex].autoProgressLines.Length > _dialogueIndex && dialogue.Lines[_dialogueIndex].autoProgressLines[_dialogueIndex])
        {
            yield return new WaitForSeconds(autoProgressDelay);
            NextLine();
        }
    }

    private async void StopVoice()
    {
        await SFXManager.StopVoice();
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        IsDialogueActive = false;
        SetDialogueText("");
        ShowDialogueUI(false);
        IsDialogueFinished = true;
        CharsInDialogue.Clear();
    }
}
