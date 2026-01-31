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
        _destination = Instantiate(_marker, new Vector2(-1.9f, 18.05f), Quaternion.identity, transform.parent);
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
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

            Fiona.Anim.SetTrigger("Talk");

            // start dialogue
            DialogueController.Instance.StartDialogue(_targetDialogue, new List<CharacterBase>{Fiona});
        }

        if (_reached && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _reached = false;
            Fiona.Anim.SetTrigger("Talk");
            EventIsDone = true; // event done
        }

        // out of bounds check
        if ((_detectPlayer && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState) || PlayerChar.Entered)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Entered = false;
            _entered = true;

            Fiona.Anim.SetTrigger("Talk");

            // start dialogue
            DialogueController.Instance.StartDialogue(_outBoundsDialogue, new List<CharacterBase>{Fiona});
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            Fiona.Anim.SetTrigger("Talk");
            StartCoroutine(GoBack());
        }

        if (_entered && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _entered = false;
            Fiona.Anim.SetTrigger("Talk");
            PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        }
    }

    private IEnumerator GoBack()
    {
        Vector3 returnDir = (Fiona.transform.position - transform.position).normalized;
        Vector3 returnPos = Fiona.transform.position;

        PlayerChar.Face(Fiona);

        // go back
        float _distance = Vector2.Distance(returnPos, PlayerChar.transform.position);
        while (_distance > 0.6f)
        {
            _distance = Vector2.Distance(returnPos, PlayerChar.transform.position);
            PlayerChar.Move(returnPos - PlayerChar.transform.position);
            yield return new WaitForFixedUpdate();
        }

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        _detectPlayer = null;
    }
}
