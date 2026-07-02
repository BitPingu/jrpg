using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToMom : EventBase
{
    public Player PlayerChar { get; set; }
    public Villager Mom { get; set; }
    [SerializeField] private GameObject _reactIcon;
    [SerializeField] private Dialogue _dialogue, _curDialogue;
    private bool _reached;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // spawn in bedroom
        PlayerChar.transform.position = new Vector3(-13.79f,-41.41f);
    }

    private void Update()
    {
        // Debug.Log("up");
        float momDistance = Vector2.Distance(Mom.transform.position, PlayerChar.transform.position);
    
        if (!_reached && momDistance < 2.5f)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Face(Mom);
            Mom.StateMachine.End();
            StartCoroutine(MoveMom());
            _reached = true;
        }
    }

    private IEnumerator MoveMom()
    {
        // react
        Vector2 iconPos = new Vector2(Mom.transform.position.x, Mom.transform.position.y+1f);
        GameObject _activeIcon = Instantiate(_reactIcon, iconPos, Quaternion.identity, PlayerChar.transform);
        yield return new WaitForSeconds(1f);
        Destroy(_activeIcon);

        // move to player
        float _distance = Vector2.Distance(PlayerChar.transform.position, Mom.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(PlayerChar.transform.position, Mom.transform.position);
            Mom.Move(PlayerChar.transform.position - Mom.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Mom.Move(Vector2.zero);

        // dialogue
        DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Mom}, false);
    }

    private void FinishEvent()
    {
        if (!_reached)
            return;

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        Mom.StateMachine.Initialize(Mom.IdleState);

        // set current mom dialogue
        Mom.CurrentDialogue = _curDialogue;

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
