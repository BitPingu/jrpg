using UnityEngine;

public class Battle : MonoBehaviour
{
    private Player _player;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>() && !hitInfo.GetComponent<Player>().Opponent)
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
        if (_player && _player.Input.E)
        {
            GetComponentInParent<Enemy>().InterruptIdle();
            StartCoroutine(_player.Engage(GetComponentInParent<Enemy>()));
            OnTriggerExit2D(_player.GetComponent<Collider2D>());
            // Destroy(this);
        }
    }
}
