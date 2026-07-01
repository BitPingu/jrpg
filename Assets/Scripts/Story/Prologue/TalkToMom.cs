using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToMom : EventBase
{
    private Player _detectPlayer;
    public Villager Mom { get; set; }
    [SerializeField] private GameObject _reactIcon;
    [SerializeField] private Dialogue _dialogue, _curDialogue;

    private void Start()
    {
        transform.position = new Vector3(3,-43.4f,0);
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
            _detectPlayer.Face(Mom);

            Mom.StateMachine.End();
            StartCoroutine(MoveMom());
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
            Mom.StateMachine.Initialize(Mom.IdleState);
            _detectPlayer = null;

            // set current mom dialogue
            Mom.CurrentDialogue = _curDialogue;

            EventIsDone = true; // event done
        }
    }

    private IEnumerator MoveMom()
    {
        Vector2 iconPos = new Vector2(Mom.transform.position.x, Mom.transform.position.y+1f);
        GameObject _activeIcon = Instantiate(_reactIcon, iconPos, Quaternion.identity, _detectPlayer.transform);
        yield return new WaitForSeconds(1f);
        Destroy(_activeIcon);

        // go
        float _distance = Vector2.Distance(_detectPlayer.transform.position, Mom.transform.position);
        while (_distance > 0.7f)
        {
            _distance = Vector2.Distance(_detectPlayer.transform.position, Mom.transform.position);
            Mom.Move(_detectPlayer.transform.position - Mom.transform.position);
            yield return new WaitForFixedUpdate();
        }

        Mom.Move(Vector2.zero);

        // dialogue
        DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{Mom}, false);
    }
}
