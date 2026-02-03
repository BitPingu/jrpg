using UnityEngine;

public class Enter : MonoBehaviour
{
    private Player _detectPlayer;
    private GameObject _activeIcon;
    [SerializeField] private Transform _location;
    [SerializeField] private bool _faceRight;
    [SerializeField] private GameObject _icon;
    [SerializeField] private AudioClip _sound;
    [SerializeField] private float _pitch = 1f;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !_detectPlayer)
        {
            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsEntering = true;

            Vector2 iconPos = new Vector2(_detectPlayer.transform.position.x, _detectPlayer.transform.position.y+1f);
            _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, _detectPlayer.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            Destroy(_activeIcon);

            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsEntering = false;
            _detectPlayer = null;
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState && !DialogueController.Instance.IsDialogueActive)
        {
            if (_detectPlayer.Input.E && _detectPlayer.CanEnter)
            {
                ScreenTransition();
            }
            else if (_detectPlayer.Input.E && !_detectPlayer.CanEnter)
            {
                _detectPlayer.Entered = true;
                _activeIcon.SetActive(false);
            }
            else if (!_detectPlayer.Entered)
            {
                _activeIcon.SetActive(true);
            }

            if (_activeIcon && !_activeIcon.activeSelf && !DialogueController.Instance.IsDialogueActive)
            {
                _activeIcon.SetActive(true);
            }
        }
        else if (_detectPlayer && DialogueController.Instance.IsDialogueActive)
        {
            _activeIcon.SetActive(false);
        }

        if (_detectPlayer && Status.Instance.IsOn)
        {
            _activeIcon.SetActive(false);
        }
    }

    private async void ScreenTransition()
    {
        Player player = _detectPlayer;
        player.StateMachine.End(); // disable movement
        _activeIcon.SetActive(false);
        await Transition.Instance.FadeOut();

        SFXManager.Play(_sound, _pitch);

        if (_faceRight)
            player.Sprite.flipX = false;
        else
            player.Sprite.flipX = true;

        // move characters
        player.transform.position = _location.position + new Vector3(_location.GetComponent<BoxCollider2D>().offset.x/2f, _location.GetComponent<BoxCollider2D>().offset.y/2f);
        if (player.CurrentCompanion)
        {
            player.CurrentCompanion.transform.position = player.transform.position;
        }

        await Transition.Instance.FadeIn();
        player.StateMachine.Initialize(player.IdleState); // enable movement
    }
}
