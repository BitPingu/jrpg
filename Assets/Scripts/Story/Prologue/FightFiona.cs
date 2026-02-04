using System.Collections.Generic;
using UnityEngine;

public class FightFiona : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _damageDialogue, _fionaDialogue, _fionaDialogue2;
    private bool _fionaHurt, _fionaDialogueFinish, _fionaDialogue2Finish;
    [SerializeField] private ItemBase _potion;

    private void Start()
    {
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState);
        Fiona.StateMachine.Initialize(Fiona.IdleState);

        Fiona.IsSparring = true;

        Fiona.Opponent = PlayerChar;
        PlayerChar.Opponent = Fiona;
    }

    private void Update()
    {
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

}
