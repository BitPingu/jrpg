using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _fionaDialogue;

    private void Start()
    {
        PlayerChar.StateMachine.End(); // disable movement
        PlayerChar.Face(Fiona);

        Fiona.Anim.enabled = false;

        // start dialogue
        DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);
    }

    private void Update()
    {
        if (DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.enabled = true;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

            // set current fiona dialogue
            // Fiona.CurrentDialogue = _curDialogue;

            EventIsDone = true; // event done
        }
    }
}
