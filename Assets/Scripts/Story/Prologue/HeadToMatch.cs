using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadToMatch : EventBase
{
    private Player _detectPlayer;
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _targetDialogue, _outBoundsDialogue;
    private bool _entered, _reached;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += OutOfBounds;
        PlayerChar.OnEnter += EnterBuilding;
        DialogueController.Instance.OnDialogueFinish += ExitBuilding;
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // boundary spawn
        transform.position = new Vector3(-2.44f,-4.76f,0);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _detectPlayer = null;
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !_detectPlayer)
        {
            _detectPlayer = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        float chiefDistance = Vector2.Distance(Chief.transform.position, PlayerChar.transform.position);

        // target reached
        if (!_reached && chiefDistance < 1.5f)
        {
            PlayerChar.StateMachine.End(); // stop movement
            Fiona.StateMachine.End(); // stop movement
            
            Fiona.Anim.Rebind();
            Fiona.Anim.enabled = false;

            // start dialogue
            DialogueController.Instance.StartDialogue(_targetDialogue, new List<CharacterBase>{Fiona}, false);

            _reached = true;
        }

        // out of bounds check
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // stop movement

            Fiona.Face(PlayerChar);
            Fiona.Anim.Rebind();
            Fiona.Anim.enabled = false;

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);
        }
    }

    private void OutOfBounds()
    {
        if (!_detectPlayer)
            return;

        Fiona.Anim.enabled = true;
        StartCoroutine(GoBack(_detectPlayer));
    }

    private IEnumerator GoBack(Player player)
    {
        player.Face(Fiona);

        Vector3 returnPos = Fiona.transform.position;

        // go back
        float _distance = Vector2.Distance(returnPos, player.transform.position);
        while (_distance > 0.6f)
        {
            _distance = Vector2.Distance(returnPos, player.transform.position);
            player.Move(returnPos - player.transform.position);
            yield return new WaitForFixedUpdate();
        }

        player.StateMachine.Initialize(player.IdleState); // enable movement
        _detectPlayer = null;
    }

    private void EnterBuilding()
    {
        // enter building check
        PlayerChar.StateMachine.End(); // stop movement

        Fiona.Face(PlayerChar);
        Fiona.Anim.Rebind();
        Fiona.Anim.enabled = false;

        // start dialogue
        DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);

        _entered = true;
    }

    private void ExitBuilding()
    {
        if (!_entered)
            return;

        _entered = false;
        Fiona.Anim.enabled = true;
        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
    }

    private void FinishEvent()
    {
        if (!_reached)
            return;

        Fiona.Anim.enabled = true;

        EventIsDone = true; // event done

        DialogueController.Instance.OnDialogueFinish -= OutOfBounds;
        PlayerChar.OnEnter -= EnterBuilding;
        DialogueController.Instance.OnDialogueFinish -= ExitBuilding;
        DialogueController.Instance.OnDialogueFinish -= FinishEvent;
    }
}
