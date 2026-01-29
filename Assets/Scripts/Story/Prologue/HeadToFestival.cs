using System.Collections;
using UnityEngine;

public class HeadToFestival : EventBase
{
    private Player _playerCol;
    public Player PlayerChar { get; set; }
    public Companion Fiona { get; set; }
    [SerializeField] private Dialogue _dialogue;
    private bool _entered;

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _playerCol = hitInfo.GetComponent<Player>();
        }
    }

    private void Update()
    {
        // out of bounds check
        if ((_playerCol && PlayerChar.StateMachine.CurrentState == PlayerChar.IdleState) || PlayerChar.Entered)
        {
            PlayerChar.StateMachine.End(); // stop movement
            PlayerChar.Entered = false;
            _entered = true;

            // start dialogue
            DialogueController.Instance.CharsInDialogue.Add(Fiona.charName, Fiona);
            DialogueController.Instance.dialogue = _dialogue;
            DialogueController.Instance.StartDialogue();
        }

        if (_playerCol && DialogueController.Instance.IsDialogueFinished)
        {
            StartCoroutine(GoBack());
            DialogueController.Instance.IsDialogueFinished = false;
        }

        if (_entered && DialogueController.Instance.IsDialogueFinished)
        {
            _entered = false;
            DialogueController.Instance.IsDialogueFinished = false;
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
