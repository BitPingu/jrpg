using UnityEngine;

public class Battle : MonoBehaviour
{
    private Player _detectPlayer;
    private GameObject _activeIcon;
    [SerializeField] private GameObject _icon;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !_detectPlayer && hitInfo.GetComponent<Player>().StateMachine.CurrentState == hitInfo.GetComponent<Player>().IdleState)
        {
            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsNearEnemy = true;

            Vector2 iconPos = new Vector2(transform.position.x, transform.position.y+.02f);
            _activeIcon = Instantiate(_icon, iconPos, Quaternion.identity, transform);
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            Destroy(_activeIcon);

            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsNearEnemy = false;
            _detectPlayer = null;
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.StateMachine.CurrentState == _detectPlayer.IdleState)
        {
            if (_detectPlayer.Input.E)
            {
                GetComponentInParent<Enemy>().InterruptIdle(); // halt enemy movement
                StartCoroutine(_detectPlayer.Engage(GetComponentInParent<Enemy>())); // engage the enemy
                OnTriggerExit2D(_detectPlayer.GetComponent<Collider2D>());
            }
        }
    }
}
