using UnityEngine;

public class TalkToFiona : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Companion _fiona;
    [SerializeField] private Dialogue _dialogue;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (_player && _player.StateMachine.CurrentState == _player.IdleState)
        {
            // player faces
            _player.StateMachine.End(); // stop movement
            _player.FaceCharacter(_fiona);

            // fiona joins
            _fiona.GetComponent<Companion>().Join(_player);

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(_fiona.charName, _fiona);
            DialogueController.Instance.CharsInDialogue.Add(_player.charName, _player);
            DialogueController.Instance.dialogue = _dialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_player && DialogueController.Instance.IsDialogueFinished)
        {
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
            DialogueController.Instance.IsDialogueFinished = false;
            Destroy(gameObject); // end event
        }
    }
}
