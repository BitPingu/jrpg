using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    private Player _player;
    private DialogueController _dialogueUI;

    public Dialogue dialogue;
    private int _dialogueIndex;
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
                // start dialogue
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

        _dialogueUI.SetNPCInfo(dialogue.npcName, dialogue.npcPortrait);
        _dialogueUI.ShowDialogueUI(true);

        GetComponentInParent<CharacterBase>().FaceCharacter(_player);
        _player.StateMachine.End(); // stop movement
        _player.FaceCharacter(GetComponentInParent<CharacterBase>());
        
        StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (_isTyping)
        {
            // skip typing animation and show the full line
            StopAllCoroutines(); // halt auto progress
            _dialogueUI.SetDialogueText(dialogue.dialogueLines[_dialogueIndex]);
            _isTyping = false;
        }
        else if (++_dialogueIndex < dialogue.dialogueLines.Length)
        {
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
        _isTyping = true;
        _dialogueUI.SetDialogueText("");

        foreach(char letter in dialogue.dialogueLines[_dialogueIndex])
        {
            _dialogueUI.SetDialogueText(_dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogue.typingSpeed);
        }

        _isTyping = false;

        // auto progress
        if (dialogue.autoProgressLines.Length > _dialogueIndex && dialogue.autoProgressLines[_dialogueIndex])
        {
            yield return new WaitForSeconds(dialogue.autoProgressDelay);
            NextLine();
        }
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
