using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    private Player _player;
    private DialogueController _dialogueUI;

    public Dialogue[] dialogue;
    private int _dialogueIndex, _currentDialogue;
    private bool _isTyping, _isDialogueActive;

    private void Start()
    {
        _dialogueUI = DialogueController.Instance;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = true;
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            _player = null;
        }
    }

    private void Update()
    {
        if (_player && _player.Input.E)
        {
            // _player.CurrentCompanion = GetComponentInParent<Companion>();
            // GetComponentInParent<Companion>().Join(_player);
            if (_isDialogueActive)
            {
                // next line
                NextLine();
            }
            else
            {
                GetComponentInParent<CharacterBase>().FaceCharacter(_player);
                _player.StateMachine.End(); // stop movement
                _player.FaceCharacter(GetComponentInParent<CharacterBase>());

                // start dialogue
                _currentDialogue = 0;
                StartDialogue();
            }
            // OnTriggerExit2D(_player.GetComponent<Collider2D>());
            // Destroy(this);
        }
    }

    private void StartDialogue()
    {
        _isDialogueActive = true;
        _dialogueIndex = 0;

        _dialogueUI.SetNPCInfo(dialogue[_currentDialogue].npcName, dialogue[_currentDialogue].npcPortrait);
        _dialogueUI.ShowDialogueUI(true);
        
        StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (_isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines(); // halt auto progress
            _dialogueUI.SetDialogueText(dialogue[_currentDialogue].dialogueLines[_dialogueIndex]);
            _isTyping = false;
            _dialogueUI.continueImage.enabled = true;
            StopVoice();
        }
        else if (++_dialogueIndex < dialogue[_currentDialogue].dialogueLines.Length)
        {
            // if another line, type next line
            StartCoroutine(TypeLine());
        }
        else if (++_currentDialogue < dialogue.Length)
        {
            // more dialogue
            StartDialogue();
        }
        else
        {
            // end dialogue
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        _dialogueUI.continueImage.enabled = false;
        _isTyping = true;
        _dialogueUI.SetDialogueText("");

        foreach(char letter in dialogue[_currentDialogue].dialogueLines[_dialogueIndex])
        {
            _dialogueUI.SetDialogueText(_dialogueUI.dialogueText.text += letter);
            SFXManager.PlayVoice(dialogue[_currentDialogue].voiceSound, dialogue[_currentDialogue].voicePitch);
            yield return new WaitForSeconds(dialogue[_currentDialogue].typingSpeed);
        }

        _isTyping = false;
        _dialogueUI.continueImage.enabled = true;
        StopVoice();

        // auto progress
        if (dialogue[_currentDialogue].autoProgressLines.Length > _dialogueIndex && dialogue[_currentDialogue].autoProgressLines[_dialogueIndex])
        {
            yield return new WaitForSeconds(dialogue[_currentDialogue].autoProgressDelay);
            NextLine();
        }
    }

    private async void StopVoice()
    {
        await SFXManager.StopVoice();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        _isDialogueActive = false;
        _dialogueUI.SetDialogueText("");
        _dialogueUI.ShowDialogueUI(false);

        _player.StateMachine.Initialize(_player.IdleState); // enable movement
        OnTriggerExit2D(_player.GetComponent<Collider2D>());
    }
}
