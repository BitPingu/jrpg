using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToFiona : EventBase
{
    private Player _detectPlayer;
    public CharacterBase Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue, _curDialogue;

    private void Start()
    {
        transform.position = new Vector3(0,0.5f,0);
    }

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

            StartCoroutine(DelayAnimStop());

            // join party
            Fiona.GetComponent<Companion>().Join(_detectPlayer);

            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.enabled = true;
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
            _detectPlayer = null;

            // set current fiona dialogue
            Fiona.CurrentDialogue = _curDialogue;

            EventIsDone = true; // event done
        }
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.enabled = false;
    }
}
