using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    private Player _player;
    private CharacterBase _character;

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
            _character = GetComponentInParent<CharacterBase>();
            // GetComponentInParent<CharacterBase>().StateMachine.End(); // stop movement
            if (_character && _character.Anim)
                _character.Anim.SetTrigger("Talk");
            _character.Face(_player);

            _player.StateMachine.End(); // disable movement
            _player.Face(_character);

            // start dialogue
            DialogueController.Instance.StartDialogue(_character.CurrentDialogue, new List<CharacterBase>{_character});
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            if (_character && _character.Anim)
                _character.Anim.SetTrigger("Talk");
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
        }
    }
}
