using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    private Player _detectPlayer;
    private GameObject _activeIcon;
    private CharacterBase _character;
    [SerializeField] private GameObject _icon;

    private void Start()
    {
        // set dialogue delegates
        DialogueController.Instance.OnDialogueFinish += End;

        if (GetComponentInParent<CharacterBase>())
        {
            _character = GetComponentInParent<CharacterBase>();
        }
    }

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
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            if (_detectPlayer.Input.E)
            {
                if (_character.GetComponent<Companion>())
                {
                    _character.Anim.Rebind(); // TODO: workaround when using back sprites
                }

                if (_character && _character.Anim)
                    _character.Anim.enabled = false;

                _detectPlayer.Face(_character);
                _character.Face(_detectPlayer);

                _detectPlayer.StateMachine.End(); // disable movement
                _character.StateMachine.End(); // disable movement

                // start dialogue
                DialogueController.Instance.StartDialogue(_character.CurrentDialogue, new List<CharacterBase>{_character}, false);
            }

            if (!_activeIcon)
            {
                // reinstantiate icon when already within bounds
                Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
                _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
            }
        }

        if (_activeIcon && DialogueController.Instance.IsDialogueActive)
        {
            // hide icon during dialogue
            _activeIcon.SetActive(false);
        }

        if (_activeIcon && !_activeIcon.activeSelf && !DialogueController.Instance.IsDialogueActive)
        {
            // reshow icon after dialogue
            _activeIcon.SetActive(true);
        }

        if (_detectPlayer && _detectPlayer.IsEntering)
        {
            OnTriggerExit2D(_detectPlayer.GetComponent<Collider2D>()); // cancel when on enter
        }

        if (_detectPlayer && _detectPlayer.StatusOn)
        {
            _activeIcon.SetActive(false);
        }
    }

    private void End()
    {
        if (!_detectPlayer)
            return;

        if (_character && _character.Anim)
            _character.Anim.enabled = true;

        _detectPlayer.StateMachine.Initialize(_detectPlayer.IdleState); // enable movement
        _character.StateMachine.Initialize(_character.IdleState); // enable movement
    }
}
