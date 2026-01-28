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
            // GetComponentInParent<CharacterBase>().StateMachine.End(); // stop movement
            GetComponentInParent<CharacterBase>().FaceCharacter(_player);

            // player faces
            _player.StateMachine.End(); // stop movement
            _player.FaceCharacter(GetComponentInParent<CharacterBase>());

            // start dialogue
            DialogueController.Instance.dialogue = GetComponentInParent<CharacterBase>().Dialogues[0];
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
