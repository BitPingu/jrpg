using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Festival : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Mom { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _chiefDialogue2, _fionaDialogue, _fionaDialogue2, _fionaDialogue3, _momDialogue;
    private int _inPos;
    private bool _chiefDialogueFinish, _fionaDialogueFinish, _fionaDialogue2Finish;

    [SerializeField] private Destination _marker;
    private Destination _destination;

    private void Start()
    {
        StartCoroutine(Go(PlayerChar, new Vector2(-1.23f, 16.99f)));
        StartCoroutine(Go(Fiona, new Vector2(-2.57f, 16.99f)));
    }

    private void Update()
    {
        if (_inPos == 2)
        {
            _inPos++;

            // start chief dialogue
            DialogueController.Instance.StartDialogue(_chiefDialogue, new List<CharacterBase>{Chief});

            Fiona.Anim.SetBool("Talk", true);

            _chiefDialogueFinish = true;
        }

        if (_chiefDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _chiefDialogueFinish = false;

            // start fiona dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona});

            _fionaDialogueFinish = true;
        }

        if (_fionaDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogueFinish = false;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
            PlayerChar.CanEnter = true; // enable enter action
            Fiona.CurrentDialogue = _fionaDialogue2; // set next dialogue
            Fiona.Anim.SetBool("Talk", false);

            Mom.CurrentDialogue = _momDialogue; // set next dialogue
            Chief.CurrentDialogue = _chiefDialogue2; // set next dialogue

            // destination marker
            _destination = Instantiate(_marker, new Vector2(-1.9f, 10.05f), Quaternion.identity, transform.parent);
        }

        // target reached
        if (_destination && _destination.Reached)
        {
            _destination.Reached = false;
            Destroy(_destination.gameObject);
            PlayerChar.StateMachine.End(); // stop movement
            // Fiona.StateMachine.End(); // stop movement
            // _reached = true;

            Fiona.Anim.SetBool("Talk", true);

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue3, new List<CharacterBase>{Fiona});

            _fionaDialogue2Finish = true;
        }

        if (_fionaDialogue2Finish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogue2Finish = false;
            EventIsDone = true; // event done
        }
    }

    private IEnumerator Go(CharacterBase character, Vector3 destination)
    {
        // go
        float _distance = Vector2.Distance(destination, character.transform.position);
        while (_distance > 0.3f)
        {
            _distance = Vector2.Distance(destination, character.transform.position);
            PlayerChar.Move(destination - character.transform.position);
            yield return new WaitForFixedUpdate();
        }

        character.Move(Vector2.zero);
        character.Face(Chief);

        _inPos++;
    }
}
