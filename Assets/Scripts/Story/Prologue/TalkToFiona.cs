using System.Collections;
using UnityEngine;

public class TalkToFiona : EventBase
{
    private Player _playerCol;
    public CharacterBase Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue, _nextDialogue;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _playerCol = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (_playerCol && _playerCol.StateMachine.CurrentState == _playerCol.IdleState)
        {
            // player faces
            _playerCol.StateMachine.End(); // stop movement
            _playerCol.Face(Fiona);

            StartCoroutine(DelayAnim());

            // fiona joins
            Fiona.GetComponent<Companion>().Join(_playerCol);

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.CharsInDialogue.Add(_playerCol.charName, _playerCol);
            DialogueController.Instance.dialogue = _dialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_playerCol && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.SetTrigger("Talk");
            _playerCol.StateMachine.Initialize(_playerCol.IdleState); // enable movement
            _playerCol.CanEnter = false; // disable enter action
            Fiona.CurrentDialogue = _nextDialogue; // set next dialogue
            EventIsDone = true; // event done
        }
    }

    private IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(1f);
        Fiona.Anim.SetTrigger("Talk");
    }
}
