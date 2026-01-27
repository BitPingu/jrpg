using UnityEngine;

public class Join : MonoBehaviour
{
    private Player _player;

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
        if (_player && _player.Input.E)
        {
            _player.CurrentCompanion = GetComponentInParent<Companion>();
            GetComponentInParent<Companion>().Join(_player);
            OnTriggerExit2D(_player.GetComponent<Collider2D>());
            Destroy(this);
        }
    }
}
