using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToFiona : EventBase
{
    public Player PlayerChar { get; set; }
    public Villager Mom { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue, _curMomDialogue;
    private bool _reached;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set current dialogue
        Mom.CurrentDialogue = _curMomDialogue;

        // spawn fiona
        Fiona.transform.position = new Vector3(1.672f,-0.557f,0);
    }

    private void Update()
    {
        float fionaDistance = Vector2.Distance(Fiona.transform.position, PlayerChar.transform.position);

        if (!_reached && fionaDistance < 2.5f && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Face(Fiona);

            StartCoroutine(DelayAnimStop());

            // join party
            Fiona.Join(PlayerChar);

            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Fiona}, false);

            _reached = true;
        }
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.enabled = false;
    }

    private void FinishEvent()
    {
        if (!_reached)
            return;

        Fiona.Anim.enabled = true;
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
