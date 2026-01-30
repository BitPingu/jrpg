using System.Collections;
using UnityEngine;

public class Festival : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Mom { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _chiefDialogue, _chiefDialogue2, _fionaDialogue, _fionaDialogue2, _fionaDialogue3, _momDialogue;
    private int _inPos;
    private bool _chiefDialogueFinish, _fionaDialogueFinish;

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
            DialogueController.Instance.CharsInDialogue.Add(Chief.charName, Chief);
            DialogueController.Instance.dialogue = _chiefDialogue;
            DialogueController.Instance.StartDialogue();

            Fiona.Anim.SetTrigger("Talk");

            _chiefDialogueFinish = true;
        }

        if (_chiefDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _chiefDialogueFinish = false;

            // start fiona dialogue
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.dialogue = _fionaDialogue;
            DialogueController.Instance.StartDialogue();

            _fionaDialogueFinish = true;
        }

        if (_fionaDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogueFinish = false;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
            PlayerChar.CanEnter = true; // enable enter action
            Fiona.CurrentDialogue = _fionaDialogue2; // set next dialogue
            Fiona.Anim.SetTrigger("Talk");

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

            Fiona.Anim.SetTrigger("Talk");

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.dialogue = _fionaDialogue3;
            DialogueController.Instance.StartDialogue();
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
        character.FaceCharacter(Chief);

        _inPos++;
    }
}
