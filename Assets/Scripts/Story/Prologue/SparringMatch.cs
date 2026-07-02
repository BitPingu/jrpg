using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparringMatch : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    public GameObject House { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _damageDialogue, _chiefDialogue2, _chiefDialogue3;
    private int _inPos;
    private bool _chiefDialogueActive, _fionaHurt, _chiefDialogue2Active, _chiefDialogue3Active;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += StartMatch;
        DialogueController.Instance.OnDialogueFinish += GiveQuest;
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        StartCoroutine(GoToBattle(PlayerChar, new Vector2(Chief.transform.position.x+.8f, Chief.transform.position.y-1f)));
        StartCoroutine(GoToBattle(Fiona, new Vector2(Chief.transform.position.x-.8f, Chief.transform.position.y-1f)));
    }

    private void Update()
    {
        // before battle
        if (_inPos == 2)
        {
            _inPos = -1;

            Fiona.Anim.Rebind();
            Fiona.Anim.enabled = false;

            // start chief dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue, new List<CharacterBase>{Chief, Fiona}, false);
            _chiefDialogueActive = true;
        }

        if (PlayerChar.Opponent && !_fionaHurt && Fiona.CurrentHealth <= Fiona.MaxHealth/2f)
        {
            // dialogue during battle
            DialogueController.Instance.StartDialogue(_damageDialogue, new List<CharacterBase>{Fiona}, true);
            _fionaHurt = true;
        }

        // after battle
        if (!PlayerChar.Opponent && Fiona.IsSparring)
        {
            PlayerChar.StateMachine.End(); // stop movement
            Fiona.Anim.Rebind();
            Fiona.Anim.enabled = false;
            Fiona.IsSparring = false;

            // start dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue2, new List<CharacterBase>{Chief, Fiona}, false);
            _chiefDialogue2Active = true;
        }
    }

    private IEnumerator GoToBattle(CharacterBase character, Vector3 destination)
    {
        // move to battle position
        float distance = Vector2.Distance(destination, character.transform.position);
        Vector2 vec = destination - character.transform.position;
        while (distance > 0.3f)
        {
            distance = Vector2.Distance(destination, character.transform.position);
            character.Move(vec);
            yield return new WaitForFixedUpdate();
        }

        character.Move(Vector2.zero);
        character.Face(Chief);

        _inPos++;
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

    private void GiveQuest()
    {
        if (!_chiefDialogue2Active)
            return;

        _chiefDialogue2Active = false;

        // start dialogue
        DialogueController.Instance.StartDialogue(_chiefDialogue3, new List<CharacterBase>{Chief, Fiona}, false);
        StartCoroutine(DelayNextDialogue());
    }

    private IEnumerator DelayNextDialogue()
    {
        yield return new WaitForSeconds(.4f);
        _chiefDialogue3Active = true;
    }

    private void FinishEvent()
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

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= StartMatch;
        DialogueController.Instance.OnDialogueFinish -= GiveQuest;
        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
