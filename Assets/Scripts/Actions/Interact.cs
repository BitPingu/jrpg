using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Dialogue _dialogue;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = true;
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            _player = null;
        }
    }

    private void Update()
    {
        if (_player && _player.Input.E && _player.StateMachine.CurrentState == _player.IdleState)
        {
            _player.StateMachine.End(); // disable movement
            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{_player});
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
        }
    }
}
