using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadToMatch : EventBase
{
    private Player _detectPlayer;
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    public Villager Chief { get; set; }
    [SerializeField] private Dialogue _curFionaDialogue, _targetDialogue, _outBoundsDialogue;
    private bool _entered, _reached;
    private int _inPos;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += OutOfBounds;
        PlayerChar.OnEnter += EnterBuilding;
        DialogueController.Instance.OnDialogueFinish += ExitBuilding;
        DialogueController.Instance.OnDialogueFinish += FinishEvent;

        // set current dialogue
        Fiona.CurrentDialogue = _curFionaDialogue;

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

        // setup before next event
        if (_inPos == 2)
        {
            _inPos = -1;

            Fiona.Anim.Rebind();
            Fiona.Anim.enabled = false;

            EventIsDone = true; // event done

            DialogueController.Instance.OnDialogueFinish -= OutOfBounds;
            PlayerChar.OnEnter -= EnterBuilding;
            DialogueController.Instance.OnDialogueFinish -= ExitBuilding;
            DialogueController.Instance.OnDialogueFinish -= FinishEvent;
        }

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

        // before battle
        StartCoroutine(GoToBattle(PlayerChar, new Vector2(Chief.transform.position.x+.8f, Chief.transform.position.y-1f)));
        StartCoroutine(GoToBattle(Fiona, new Vector2(Chief.transform.position.x-.8f, Chief.transform.position.y-1f)));
    }

    private IEnumerator GoToBattle(CharacterBase character, Vector3 destination)
    {
        // move to battle position
        float distance = Vector2.Distance(destination, character.transform.position);
        Vector2 vec = destination - character.transform.position;
        while (distance > 0.1f)
        {
            distance = Vector2.Distance(destination, character.transform.position);
            character.Move(vec);
            yield return new WaitForFixedUpdate();
        }

        character.Move(Vector2.zero);
        character.Face(Chief);

        _inPos++;
    }
}
