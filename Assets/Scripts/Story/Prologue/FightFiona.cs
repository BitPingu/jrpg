using System.Collections.Generic;
using UnityEngine;

public class FightFiona : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _fightDialogue, _fionaDialogue, _fionaDialogue2;
    private bool _fionaHurt, _fionaDialogueFinish, _fionaDialogue2Finish;
    [SerializeField] private Destination _marker;
    private Destination _destination;

    private void Start()
    {
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);

        Fiona.Sparring = true;
        Fiona.transform.Find("Talk").gameObject.SetActive(false);

        Fiona.Opponent = PlayerChar;
        PlayerChar.Opponent = Fiona;
    }

    private void Update()
    {
        if (PlayerChar.Opponent && !_fionaHurt && Fiona.CurrentHealth <= Fiona.MaxHealth/2f)
        {
            // start dialogue
            DialogueController.Instance.StartDialogue(_fightDialogue, new List<CharacterBase>{Fiona}, true);
            _fionaHurt = true;
        }

        if (!PlayerChar.Opponent && Fiona.Sparring)
        {
            Fiona.Sparring = false;

            PlayerChar.StateMachine.End(); // stop movement

            // reset
            Fiona.CurrentHealth = Fiona.MaxHealth;

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);

            _fionaDialogueFinish = true;
        }

        if (_fionaDialogueFinish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogueFinish = false;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
            Fiona.StateMachine.Initialize(Fiona.IdleState); // enable movement
            Fiona.Anim.SetBool("Talk", false);

            // destination marker
            _destination = Instantiate(_marker, new Vector2(10f, 18.05f), Quaternion.identity, transform.parent);
        }

        // target reached
        if (_destination && _destination.Reached)
        {
            _destination.Reached = false;
            Destroy(_destination.gameObject);
            PlayerChar.StateMachine.End(); // stop movement

            Fiona.Anim.SetBool("Talk", true);

            // start dialogue
            DialogueController.Instance.StartDialogue(_fionaDialogue2, new List<CharacterBase>{Fiona}, false);

            _fionaDialogue2Finish = true;
        }

        if (_fionaDialogue2Finish && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _fionaDialogue2Finish = false;

            Fiona.Anim.SetTrigger("Talk");

            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

            EventIsDone = true; // event done
        }
    }

}
