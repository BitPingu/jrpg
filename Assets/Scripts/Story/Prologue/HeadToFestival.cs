using System.Collections;
using UnityEngine;

public class HeadToFestival : EventBase
{
    private Player _playerCol;
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _targetDialogue;
    [SerializeField] private Dialogue _outBoundsDialogue;
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
            _playerCol = hitInfo.GetComponent<Player>();
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
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.dialogue = _targetDialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_reached && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _reached = false;
            Fiona.Anim.SetTrigger("Talk");
            EventIsDone = true; // event done
        }

        // out of bounds check
        if ((_playerCol && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState) || PlayerChar.Entered)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Entered = false;
            _entered = true;

            Fiona.Anim.SetTrigger("Talk");

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.dialogue = _outBoundsDialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_playerCol && DialogueController.Instance.IsDialogueFinished)
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

        PlayerChar.FaceCharacter(Fiona);

        // go back
        float _distance = Vector2.Distance(returnPos, PlayerChar.transform.position);
        while (_distance > 0.6f)
        {
            _distance = Vector2.Distance(returnPos, PlayerChar.transform.position);
            PlayerChar.Move(returnPos - PlayerChar.transform.position);
            yield return new WaitForFixedUpdate();
        }

        PlayerChar.StateMachine.Initialize(PlayerChar.IdleState); // enable movement
        _playerCol = null;
    }
}
