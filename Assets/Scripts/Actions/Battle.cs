using UnityEngine;

public class Battle : MonoBehaviour
{
    private Player _detectPlayer;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && hitInfo.GetComponent<Player>().StateMachine.CurrentState == hitInfo.GetComponent<Player>().IdleState)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsNearEnemy = true;
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            GetComponent<SpriteRenderer>().enabled = false;
            _detectPlayer = hitInfo.GetComponent<Player>();
            _detectPlayer.IsNearEnemy = false;
            _detectPlayer = null;
        }
    }

    private void Update()
    {
        if (_detectPlayer && _detectPlayer.Input.E)
        {
            GetComponentInParent<Enemy>().InterruptIdle();
            StartCoroutine(_detectPlayer.Engage(GetComponentInParent<Enemy>()));
            OnTriggerExit2D(_detectPlayer.GetComponent<Collider2D>());
            // Destroy(this);
        }
    }
}
