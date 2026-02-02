using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadToFestival : EventBase
{
    private Player _detectPlayer;
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _targetDialogue, _outBoundsDialogue;
    private bool _entered, _reached;

    [SerializeField] private Destination _marker;
    private Destination _destination;

    private void Start()
    {
        // destination marker
        _destination = Instantiate(_marker, new Vector2(12.48f,8.09f), Quaternion.identity, transform.parent);
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
        // target reached
        if (_destination.Reached)
        {
            _destination.Reached = false;
            Destroy(_destination.gameObject);
            PlayerChar.StateMachine.End(); // stop movement
            // Fiona.StateMachine.End(); // stop movement
            _reached = true;

            Fiona.Anim.SetBool("Talk", true);

            // start dialogue
            DialogueController.Instance.StartDialogue(_targetDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_reached && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _reached = false;
            Fiona.Anim.SetBool("Talk", false);
            EventIsDone = true; // event done
        }

        // out of bounds check
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // stop movement
            // _entered = true;

            Fiona.Anim.SetBool("Talk", true);

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.SetBool("Talk", false);
            StartCoroutine(GoBack(_detectPlayer));
        }

        // enter building check
        if (!_entered && PlayerChar.Entered)
        {
            PlayerChar.StateMachine.End(); // stop movement
            _entered = true;

            Fiona.Anim.SetBool("Talk", true);

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona}, false);
        }

        if (_entered && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            PlayerChar.Entered = false;
            _entered = false;
            Fiona.Anim.SetBool("Talk", false);
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        }
    }

    private IEnumerator GoBack(Player player)
    {
        Vector3 returnDir = (Fiona.transform.position - transform.position).normalized;
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
}
