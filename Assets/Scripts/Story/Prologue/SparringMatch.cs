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
    private bool _chiefDialogueFinish, _matchStart, _fionaHurt, _chiefDialogue2Finish, _chiefDialogue3Finish;

    private void Start()
    {
        StartCoroutine(GoToBattle(PlayerChar, new Vector2(Chief.transform.position.x+.8f, Chief.transform.position.y-1f)));
        StartCoroutine(GoToBattle(Fiona, new Vector2(Chief.transform.position.x-.8f, Chief.transform.position.y-1f)));
    }

    private void Update()
    {
        if (_inPos == 2)
        {
            _inPos++;

            // start chief dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue, new List<CharacterBase>{Chief, Fiona}, false);

            StartCoroutine(DelayAnimStop());

            _chiefDialogueFinish = true;
        }

        if (!_matchStart && _chiefDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            // start the match
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
            Fiona.StateMachine.Initialize(Fiona.IdleState);

            Fiona.Anim.enabled = true;
            Fiona.IsSparring = true;

            PlayerChar.Opponent = Fiona;
            Fiona.Opponent = PlayerChar;

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

            PlayerChar.StateMachine.End(); // stop movement

            StartCoroutine(DelayAnimStop());
            Fiona.IsSparring = false;

            // start dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue2, new List<CharacterBase>{Chief, Fiona}, false);

            _chiefDialogue2Finish = true;
        }

        if (_chiefDialogue2Finish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _chiefDialogue2Finish = false;

            // start dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue3, new List<CharacterBase>{Chief, Fiona}, false);

            _chiefDialogue3Finish = true;
        }

        if (_chiefDialogue3Finish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _chiefDialogue3Finish = false;

            Chief.StateMachine.End();
            StartCoroutine(MoveChief());
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
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.Rebind();
        Fiona.Anim.enabled = false;
    }
}
