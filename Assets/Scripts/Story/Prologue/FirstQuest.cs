using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Mom { get; set; }
    [SerializeField] private Dialogue _fionaDialogue, _momDialogue;
    private bool _fionaDialogueActive;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // reset mom position
        Mom.transform.position = new Vector3(.73f,-43.48f,0);

        Fiona.Anim.enabled = true;
        StartCoroutine(MoveFiona());
    }

    private void Update()
    {
        
    }

    private IEnumerator MoveFiona()
    {
        // move to player
        float _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(PlayerChar.transform.position, Fiona.transform.position);
            Fiona.Move(PlayerChar.transform.position - Fiona.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Fiona.Move(Vector2.zero);

        // start dialogue
        DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);
        _fionaDialogueActive = true;
    }

    private void FinishEvent()
    {
        if (!_fionaDialogueActive)
            return;

        _fionaDialogueActive = false;

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

        // set current fiona dialogue
        // Fiona.CurrentDialogue = _curDialogue;

        Mom.CurrentDialogue = _momDialogue;

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
