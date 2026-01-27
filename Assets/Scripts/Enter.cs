using UnityEngine;

public class Enter : MonoBehaviour
{
    [SerializeField] private GameObject _enterIcon;
    private GameObject _enterIconRef;

    private Player _player;
    private bool _reset = true;

    [SerializeField] private Transform _location;

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            if (_reset)
            {
                _enterIconRef = Instantiate(_enterIcon, transform);
                _reset = false;
            }
            _player = hitInfo.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D hitInfo)
    {
        if (hitInfo.GetComponent<Player>())
        {
            Destroy(_enterIconRef);
            _player = null;
            _reset = true;
        }
    }

    private void Update()
    {
        if (_player && _player.Input.E)
        {
            _player.transform.position = _location.position;
            Debug.Log(_player.name + " entered a building.");
        }
    }
}
