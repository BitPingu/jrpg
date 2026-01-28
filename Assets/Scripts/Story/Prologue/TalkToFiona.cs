using UnityEngine;

public class TalkToFiona : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Companion _fiona;

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
            DialogueController.Instance.dialogue = _fiona.Dialogues[0];
            DialogueController.Instance.StartDialogue();
            Destroy(gameObject);
        }
    }
}
