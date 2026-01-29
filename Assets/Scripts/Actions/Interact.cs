using System.Collections;
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
            _player.StateMachine.End(); // stop movement

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(_player.charName, _player);
            DialogueController.Instance.dialogue = _dialogue;
            DialogueController.Instance.DelaySkip = true;
            StartCoroutine(DelaySkip());
            DialogueController.Instance.StartDialogue();
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
        }
    }

    private IEnumerator DelaySkip()
    {
        yield return new WaitForSeconds(.1f);
        DialogueController.Instance.DelaySkip = false;
    }
}
