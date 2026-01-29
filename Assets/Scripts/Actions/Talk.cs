using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    protected Player _player;

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
            CharacterBase character = GetComponentInParent<CharacterBase>();
            // GetComponentInParent<CharacterBase>().StateMachine.End(); // stop movement
            character.FaceCharacter(_player);

            // player faces
            _player.StateMachine.End(); // stop movement
            _player.FaceCharacter(character);

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(character.charName, character);
            DialogueController.Instance.dialogue = character.CurrentDialogue;
            DialogueController.Instance.DelaySkip = true;
            StartCoroutine(DelaySkip());
            DialogueController.Instance.StartDialogue();
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
            DialogueController.Instance.IsDialogueFinished = false;
        }
    }

    private IEnumerator DelaySkip()
    {
        yield return new WaitForSeconds(.1f);
        DialogueController.Instance.DelaySkip = false;
    }
}
