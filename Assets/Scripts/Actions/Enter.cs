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
            GetComponent<SpriteRenderer>().enabled = true;
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            _player = null;
        }
    }

    private void Update()
    {
        if (_player && _player.Input.E && _player.StateMachine.CurrentState == _player.IdleState)
        {
            ScreenTransition();
        }
    }

    private async void ScreenTransition()
    {
        Player player = _player;
        player.StateMachine.End(); // stop movement
        await Transition.Instance.FadeOut();
        SFXManager.Play(_sound, _pitch);
        player.transform.position = _location.position + new Vector3(_location.GetComponent<BoxCollider2D>().offset.x/2f, _location.GetComponent<BoxCollider2D>().offset.y/2f);
        await Transition.Instance.FadeIn();
        player.StateMachine.Initialize(player.IdleState); // enable movement
    }
}
