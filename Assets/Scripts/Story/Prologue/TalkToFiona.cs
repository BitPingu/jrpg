using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToFiona : EventBase
{
    private Player _detectPlayer;
    public CharacterBase Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue, _nextDialogue;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _detectPlayer = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // disable movement
            _detectPlayer.Face(Fiona);

            StartCoroutine(DelayAnim());
            Fiona.GetComponent<Companion>().Join(_detectPlayer);

            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Fiona, _detectPlayer}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.SetBool("Talk", false);
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
            _detectPlayer.CanEnter = false; // disable enter action
            Fiona.CurrentDialogue = _nextDialogue;
            _detectPlayer = null;
            EventIsDone = true; // event done
        }
    }

    private IEnumerator DelayAnim()
    {
        yield return new WaitForSeconds(1f);
        Fiona.Anim.SetBool("Talk", true);
    }
}
