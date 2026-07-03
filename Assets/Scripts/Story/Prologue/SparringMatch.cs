using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparringMatch : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    public GameObject House { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _damageDialogue, _chiefDialogue2, _chiefDialogue3, _fionaDialogue;
    private bool _chiefDialogueActive, _fionaHurt, _chiefDialogue2Active, _chiefDialogue3Active, _fionaDialogueActive;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += StartMatch;
        DialogueController.Instance.OnBattleDialogueFinish += FinishMatch;
        DialogueController.Instance.OnDialogueFinish += GiveQuest;
        DialogueController.Instance.OnDialogueFinish += ChiefReturns;
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set spawn
        PlayerChar.transform.position = new Vector2(Chief.transform.position.x+.8f, Chief.transform.position.y-1f);
        Fiona.transform.position = new Vector2(Chief.transform.position.x-.8f, Chief.transform.position.y-1f);

        // end states
        if (PlayerChar.StateMachine.CurrentState != null)
            PlayerChar.StateMachine.End(); // stop movement
        if (Fiona.StateMachine.CurrentState != null)
            Fiona.StateMachine.End(); // stop movement
        
        PlayerChar.Face(Chief);
        Fiona.Face(Chief);

        Fiona.Anim.enabled = false;

        // start chief dialogue
        DialogueController.Instance.StartDialogue(_chiefDialogue, new List<CharacterBase>{Chief, Fiona});
        _chiefDialogueActive = true;
    }

    private void Update()
    {
        if (PlayerChar.Opponent && !_fionaHurt && Fiona.CurrentHealth <= Fiona.MaxHealth/2f)
        {
            // dialogue during battle
            SecondaryDialogueController.Instance.StartDialogue(_damageDialogue, new List<CharacterBase>{Fiona});
            _fionaHurt = true;
        }
    }

    private void StartMatch()
    {
        if (!_chiefDialogueActive)
            return;

        _chiefDialogueActive = false;

        // start the match
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);

        Fiona.Anim.enabled = true;
        Fiona.IsSparring = true;

        PlayerChar.Opponent = Fiona;
        Fiona.Opponent = PlayerChar;
    }

    private void FinishMatch()
    {
        if (!Fiona.IsSparring)
        {
            return;
        }

        PlayerChar.Opponent = null;
        Fiona.Opponent = null;

        Fiona.IsSparring = false;

        PlayerChar.StateMachine.End(); // stop movement (and combat)
        Debug.Log("opponent?" + PlayerChar.Opponent + " and " + Fiona.Opponent);

        // start dialogue
        DialogueController.Instance.StartDialogue(_chiefDialogue2, new List<CharacterBase>{Chief, Fiona});
        _chiefDialogue2Active = true;
    }

    private void GiveQuest()
    {
        if (!_chiefDialogue2Active)
            return;

        _chiefDialogue2Active = false;

        // start dialogue
        DialogueController.Instance.StartDialogue(_chiefDialogue3, new List<CharacterBase>{Chief, Fiona});
        StartCoroutine(DelayNextDialogue());
    }

    private IEnumerator DelayNextDialogue()
    {
        yield return new WaitForSeconds(.4f);
        _chiefDialogue3Active = true;
    }

    private void ChiefReturns()
    {
        if (!_chiefDialogue3Active)
            return;

        _chiefDialogue3Active = false;

        Chief.StateMachine.End();
        StartCoroutine(MoveChief());
    }

    private IEnumerator MoveChief()
    {
        // move to house
        float _distance = Vector2.Distance(House.transform.position, Chief.transform.position);

        while (_distance > 0.1f)
        {
            _distance = Vector2.Distance(House.transform.position, Chief.transform.position);
            Chief.Move(House.transform.position - Chief.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Chief.Move(Vector2.zero);
        Chief.gameObject.SetActive(false); // TODO: temp before moving to house

        Fiona.Anim.enabled = true;
        StartCoroutine(MoveFiona());
    }

    private IEnumerator MoveFiona()
    {
        // move to player
        float _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
            Fiona.Move(PlayerChar.transform.position - Fiona.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Fiona.Move(Vector2.zero);

        // start dialogue
        DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona});
        _fionaDialogueActive = true;
    }

    private void FinishEvent()
    {
        if (!_fionaDialogueActive)
            return;

        _fionaDialogueActive = false;

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= StartMatch;
        DialogueController.Instance.OnDialogueFinish -= GiveQuest;
        DialogueController.Instance.OnDialogueFinish -= ChiefReturns;
        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
