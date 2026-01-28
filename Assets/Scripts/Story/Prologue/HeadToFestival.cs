using System.Collections;
using UnityEngine;

public class HeadToFestival : MonoBehaviour
{
    private Player _player;
    [SerializeField] Companion _fiona;
    [SerializeField] private Dialogue _dialogue;

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        // out of bounds check
        if (_player && _player.StateMachine.CurrentState == _player.IdleState)
        {
            _player.StateMachine.End(); // stop movement

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(_fiona.charName, _fiona);
            DialogueController.Instance.dialogue = _dialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            StartCoroutine(GoBack());
            DialogueController.Instance.IsDialogueFinished = false;
        }
    }

    private IEnumerator GoBack()
    {
        Vector3 returnDir = (_fiona.transform.position - transform.position).normalized;
        Vector3 returnPos = _fiona.transform.position;

        // go back
        float _distance = Vector2.Distance(returnPos, _player.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(returnPos, _player.transform.position);
            _player.Move(returnPos - _player.transform.position);
            yield return new WaitForFixedUpdate();
        }

        _player.FaceCharacter(_fiona);
        _player.StateMachine.Initialize(_player.IdleState); // enable movement
        _player = null;
    }
}
