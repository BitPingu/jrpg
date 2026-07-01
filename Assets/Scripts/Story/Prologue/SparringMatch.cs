using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparringMatch : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _damageDialogue, _fionaDialogue, _fionaDialogue2;
    private int _inPos;
    private bool _chiefDialogueFinish, _matchStart, _fionaHurt, _fionaDialogueFinish, _fionaDialogue2Finish;
    [SerializeField] private ItemBase _potion;

    private void Start()
    {
        StartCoroutine(Go(PlayerChar, new Vector2(Chief.transform.position.x+.8f, Chief.transform.position.y-1f)));
        StartCoroutine(Go(Fiona, new Vector2(Chief.transform.position.x-.8f, Chief.transform.position.y-1f)));
    }

    private void Update()
    {
        if (_inPos == 2)
        {
            _inPos++;

            // start chief dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue, new List<CharacterBase>{Chief}, false);

            Fiona.Anim.SetBool("Talk", true);

            _chiefDialogueFinish = true;
        }

        if (!_matchStart && _chiefDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            Fiona.Anim.SetBool("Talk", false);
            // start the match
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
            Fiona.StateMachine.Initialize(Fiona.IdleState);

            Fiona.IsSparring = true;

            Fiona.Opponent = PlayerChar;
            PlayerChar.Opponent = Fiona;

            _matchStart = true;
        }

        if (PlayerChar.Opponent && !_fionaHurt && Fiona.CurrentHealth <= Fiona.MaxHealth/2f)
        {
            // start dialogue
            DialogueController.Instance.StartDialogue(_damageDialogue, new List<CharacterBase>{Fiona}, true);
            _fionaHurt = true;
        }

        if (!PlayerChar.Opponent && Fiona.IsSparring)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.IsSparring = false;

            PlayerChar.StateMachine.End(); // stop movement

            // reset
            Fiona.CurrentHealth = Fiona.MaxHealth;
            Fiona.HBar.UpdateBar(Fiona.MaxHealth, Fiona.CurrentHealth);

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);

            _fionaDialogueFinish = true;
        }

        if (_fionaDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogueFinish = false;

            // give item
            InventoryController.Instance.AddItem(_potion);

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue2, new List<CharacterBase>{Fiona}, false);

            _fionaDialogue2Finish = true;
        }

        if (_fionaDialogue2Finish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogue2Finish = false;

            Fiona.transform.Find("Talk").gameObject.SetActive(true);
            Fiona.Anim.SetBool("Talk", false);

            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
            Fiona.StateMachine.Initialize(Fiona.IdleState); // enable movement

            EventIsDone = true; // event done
        }
    }

    private IEnumerator Go(CharacterBase character, Vector3 destination)
    {
        // go
        float distance = Vector2.Distance(destination, character.transform.position);
        Vector2 vec = destination - character.transform.position;
        while (distance > 0.1f)
        {
            distance = Vector2.Distance(destination, character.transform.position);
            character.Move(vec);
            yield return new WaitForFixedUpdate();
        }

        character.Move(Vector2.zero);
        character.Face(Chief);

        _inPos++;
    }
}
