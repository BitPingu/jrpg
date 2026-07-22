using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToFriend : EventBase
{
    public Player PlayerChar { get; set; }
    public Villager Mom { get; set; }
    public Companion Friend { get; set; }
    [SerializeField] private Dialogue _dialogue, _curMomDialogue;
    private bool _reached;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set current dialogue
        Mom.CurrentDialogue = _curMomDialogue;

        // spawn friend
        Friend.transform.position = new Vector3(3f,-.3f);
    }

    private void Update()
    {
        float friendDistance = Vector2.Distance(Friend.transform.position, PlayerChar.transform.position);

        if (!_reached && friendDistance < 2.5f && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Face(Friend);

            StartCoroutine(DelayAnimStop());

            // join party
            Friend.Join(PlayerChar);

            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Friend});

            _reached = true;
        }
    }

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Friend.Anim.enabled = false;
    }

    private void FinishEvent()
    {
        if (!_reached)
            return;

        Friend.Anim.enabled = true;
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
