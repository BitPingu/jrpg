using UnityEngine;

public class Enter : MonoBehaviour
{
    private Player _player;
    [SerializeField] private Transform _location;

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
            Debug.Log(_player.name + " entered a building.");
        }
    }

    async void ScreenTransition()
    {
        Player player = _player;
        player.StateMachine.End();
        await Transition.Instance.FadeOut();
        player.transform.position = _location.position + new Vector3(_location.GetComponent<BoxCollider2D>().offset.x, _location.GetComponent<BoxCollider2D>().offset.y);
        await Transition.Instance.FadeIn();
        player.StateMachine.Initialize(player.IdleState);
    }
}
