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
            _reached = true;

            StartCoroutine(DelayAnimStop());

            // start dialogue
            DialogueController.Instance.StartDialogue(_targetDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_reached && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _reached = false;
            Fiona.Anim.enabled = true;
            EventIsDone = true; // event done
        }

        // out of bounds check
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // stop movement

            StartCoroutine(DelayAnimStop());

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.enabled = true;
            StartCoroutine(GoBack(_detectPlayer));
        }

        // enter building check
        if (!_entered && PlayerChar.Entered)
        {
            PlayerChar.StateMachine.End(); // stop movement
            _entered = true;

            StartCoroutine(DelayAnimStop());

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_entered && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            PlayerChar.Entered = false;
            _entered = false;
            Fiona.Anim.enabled = true;
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        }
    }

    private IEnumerator GoBack(Player player)
    {
        Vector3 returnPos = Fiona.transform.position;

        player.Face(Fiona);

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

    private IEnumerator DelayAnimStop()
    {
        yield return new WaitForSeconds(.4f);
        Fiona.Anim.enabled = false;
    }
}
