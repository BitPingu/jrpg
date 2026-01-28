using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } // Singleton instance

    public Dialogue dialogue;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage, continueImage;

    private int _dialogueIndex = 0, _currentDialogue = 0;
    public bool IsDialogueActive { get; set; }
    private bool _isTyping;

    public bool DelaySkip { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show); // Toggle UI visability
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
        if (GameObject.Find("Player").GetComponent<Player>().Input.E && IsDialogueActive && !DelaySkip)
        {
            // next line
            NextLine();
        }
    }

    public void StartDialogue()
    {
        IsDialogueActive = true;
        _dialogueIndex = 0;

        SetNPCInfo(dialogue.npcName, dialogue.npcPortrait);
        ShowDialogueUI(true);
        
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (_isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines(); // halt auto progress
            SetDialogueText(dialogue.dialogueLines[_dialogueIndex]);
            _isTyping = false;
            continueImage.enabled = true;
            StopVoice();
        }
        else if (++_dialogueIndex < dialogue.dialogueLines.Length)
        {
            // if another line, type next line
            StartCoroutine(TypeLine());
        }
        // else if (++_currentDialogue < dialogue.Length)
        // {
        //     // more dialogue
        //     StartDialogue();
        // }
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

        foreach(char letter in dialogue.dialogueLines[_dialogueIndex])
        {
            SetDialogueText(dialogueText.text += letter);
            SFXManager.PlayVoice(dialogue.voiceSound, dialogue.voicePitch);
            yield return new WaitForSeconds(dialogue.typingSpeed);
        }

        _isTyping = false;
        continueImage.enabled = true;
        StopVoice();

        // auto progress
        if (dialogue.autoProgressLines.Length > _dialogueIndex && dialogue.autoProgressLines[_dialogueIndex])
        {
            yield return new WaitForSeconds(dialogue.autoProgressDelay);
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
        GameObject.Find("Player").GetComponent<Player>().StateMachine.Initialize(GameObject.Find("Player").GetComponent<Player>().IdleState); // enable movement
    }
}
