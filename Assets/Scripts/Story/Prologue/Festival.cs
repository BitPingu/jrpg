using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Festival : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Mom { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _fionaDialogue, _fionaDialogue2;
    private int _inPos;
    private bool _chiefDialogueFinish, _fionaDialogueFinish, _fionaDialogue2Finish;

    [SerializeField] private Destination _marker;
    private Destination _destination;

    private void Start()
    {
        StartCoroutine(Go(PlayerChar, new Vector2(14.3f, 4.69f)));
        StartCoroutine(Go(Fiona, new Vector2(13.489f, 4.27f)));
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

        if (_chiefDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _chiefDialogueFinish = false;

            // start fiona dialogue
            Fiona.Face(PlayerChar);
            DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);

            _fionaDialogueFinish = true;
        }

        if (_fionaDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogueFinish = false;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
            PlayerChar.CanEnter = true; // enable enter action
            Fiona.Anim.SetBool("Talk", false);

            // destination marker
            _destination = Instantiate(_marker, new Vector2(15f, 4f), Quaternion.identity, transform.parent);
        }

        // target reached
        if (_destination && _destination.Reached)
        {
            _destination.Reached = false;
            Destroy(_destination.gameObject);
            PlayerChar.StateMachine.End(); // stop movement
            if (Fiona.StateMachine.CurrentState != null)
                Fiona.StateMachine.End(); // stop movement

            Fiona.Anim.SetBool("Talk", true);
            Fiona.transform.Find("Talk").gameObject.SetActive(false);

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue2, new List<CharacterBase>{Fiona}, false);

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
