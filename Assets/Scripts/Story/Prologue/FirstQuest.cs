using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuest : EventBase
{
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _fionaDialogue;

    private void Start()
    {
        Fiona.Anim.enabled = true;
        StartCoroutine(MoveFiona());
    }

    private void Update()
    {
        if (DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

            // set current fiona dialogue
            // Fiona.CurrentDialogue = _curDialogue;

            EventIsDone = true; // event done
        }
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

        // dialogue
        DialogueController.Instance.StartDialogue(_fionaDialogue, new List<CharacterBase>{Fiona}, false);
    }
}
