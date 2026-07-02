using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToFiona : EventBase
{
    public Player PlayerChar { get; set; }
    public CharacterBase Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue, _curDialogue;
    private bool _reached;

    private void Update()
    {
        float fionaDistance = Vector2.Distance(Fiona.transform.position, PlayerChar.transform.position);

        if (!_reached && fionaDistance < 2.5f && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Face(Fiona);

            StartCoroutine(DelayAnimStop());

            // join party
            Fiona.GetComponent<Companion>().Join(PlayerChar);

            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Fiona}, false);

            _reached = true;
        }

        if (_reached && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;

            Fiona.Anim.enabled = true;

            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

            // set current fiona dialogue
            Fiona.CurrentDialogue = _curDialogue;

            EventIsDone = true; // event done
        }
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.enabled = false;
    }
}
