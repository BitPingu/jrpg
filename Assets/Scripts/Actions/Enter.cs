using UnityEngine;

public class Enter : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Transform _location;
    [SerializeField] private AudioClip _sound;
    [SerializeField] private float _pitch = 1f;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            _player = null;
        }
    }

    private void Update()
    {
        if (_player && _player.StateMachine.CurrentState == _player.IdleState)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            if (_player.Input.E && _player.CanEnter)
                ScreenTransition();
            else if (_player.Input.E && !_player.CanEnter)
                _player.Entered = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private async void ScreenTransition()
    {
        Player player = _player;
        player.StateMachine.End(); // stop movement
        await Transition.Instance.FadeOut();

        SFXManager.Play(_sound, _pitch);

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
