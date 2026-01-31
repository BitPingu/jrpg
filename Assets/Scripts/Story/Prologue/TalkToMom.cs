using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToMom : EventBase
{
    private Player _detectPlayer;
    public Villager Mom { get; set; }
    [SerializeField] private Dialogue _dialogue, _dialogue2;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _detectPlayer = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // disable movement
            _detectPlayer.Face(Mom);

            Mom.StateMachine.End();
            StartCoroutine(MoveMom());
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            Mom.CurrentDialogue = _dialogue2;
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
            Mom.StateMachine.Initialize(Mom.IdleState);
            _detectPlayer = null;
            EventIsDone = true; // event done
        }
    }

    private IEnumerator MoveMom()
    {
        // go
        float _distance = Vector2.Distance(_detectPlayer.transform.position, Mom.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(_detectPlayer.transform.position, Mom.transform.position);
            Mom.Move(_detectPlayer.transform.position - Mom.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Mom.Move(Vector2.zero);

        // dialogue
        DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Mom});
    }
}
