using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    private Player _detectPlayer;
    private GameObject _activeIcon;
    private CharacterBase _character;
    [SerializeField] private GameObject _icon;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !_detectPlayer)
        {
            _detectPlayer = hitInfo.GetComponent<Player>();

            Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
            if (_detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
            {
                _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
            }
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
            _character = GetComponentInParent<CharacterBase>();
            // GetComponentInParent<CharacterBase>().StateMachine.End(); // disable movement

            if (_activeIcon)
            {
                _activeIcon.SetActive(false);
            }

            if (_character && _character.Anim)
                _character.Anim.SetBool("Talk", true);
            _character.Face(_detectPlayer);

            _detectPlayer.StateMachine.End(); // disable movement
            _detectPlayer.Face(_character);

            // start dialogue
            DialogueController.Instance.StartDialogue(_character.CurrentDialogue, new List<CharacterBase>{_character}, false);
        }

        if (_detectPlayer && DialogueController.Instance.IsDialogueFinished)
        {
            DialogueController.Instance.IsDialogueFinished = false;
            if (_activeIcon)
            {
                _activeIcon.SetActive(true);
            }
            else
            {
                Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
                _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
            }
            if (_character && _character.Anim)
                _character.Anim.SetBool("Talk", false);
            _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
        }

        if (_detectPlayer && _detectPlayer.IsEntering)
        {
            OnTriggerExit2D(_detectPlayer.GetComponent<Collider2D>()); // cancel when on enter
        }

        if (_detectPlayer && !_activeIcon && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
            _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
        }
    }
}
