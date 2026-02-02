using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private Player _detectPlayer;
    private GameObject _activeIcon;
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private GameObject _icon;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !_detectPlayer)
        {
            _detectPlayer = hitInfo.GetComponent<Player>();

            Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
            _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            Destroy(_activeIcon);

            _detectPlayer = null;
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.Input.E && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            _detectPlayer.StateMachine.End(); // disable movement
            _activeIcon.SetActive(false);
            // start dialogue
            DialogueController.Instance.StartDialogue(_dialogue, new List<CharacterBase>{_detectPlayer}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            _activeIcon.SetActive(true);
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
        }
    }
}
