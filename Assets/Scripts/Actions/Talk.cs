using System.Collections;
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
            // face player
            _character = GetComponentInParent<CharacterBase>();
            // GetComponentInParent<CharacterBase>().StateMachine.End(); // stop movement
            if (_character && _character.Anim)
                _character.Anim.SetTrigger("Talk");
            _character.FaceCharacter(_player);

            // player faces
            _player.StateMachine.End(); // stop movement
            _player.FaceCharacter(_character);

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(_character.charName, _character);
            DialogueController.Instance.dialogue = _character.CurrentDialogue;
            DialogueController.Instance.DelaySkip = true;
            StartCoroutine(DelaySkip());
            DialogueController.Instance.StartDialogue();
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            if (_character && _character.Anim)
                _character.Anim.SetTrigger("Talk");
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
        }
    }

    private IEnumerator DelaySkip()
    {
        yield return new WaitForSeconds(.1f);
        DialogueController.Instance.DelaySkip = false;
    }
}
