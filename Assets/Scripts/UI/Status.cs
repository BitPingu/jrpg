using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; } // Singleton instance

    [SerializeField] private PlayerController _input;
    [SerializeField] private Player _player;
    public bool IsOn { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    private void Update()
    {
        if (_input.Q && _player.StateMachine.CurrentState == _player.IdleState && !DialogueController.Instance.IsDialogueActive)
        {
            _player.StateMachine.End(); // disable movement
            IsOn = true;
        }
        else if (_input.Q && IsOn)
        {
            _player.StateMachine.Initialize(_player.IdleState); // enable movement
            IsOn = false;
        }

        if (IsOn)
        {
            // show bars
            _player.HBar.gameObject.GetComponent<Image>().enabled = true;
            _player.EBar.gameObject.GetComponent<Image>().enabled = true;
            if (_player.CurrentCompanion)
            {
                _player.CurrentCompanion.HBar.gameObject.GetComponent<Image>().enabled = true;
                _player.CurrentCompanion.EBar.gameObject.GetComponent<Image>().enabled = true;
            }
        }
        else if (!IsOn && _player.StateMachine.CurrentState == _player.IdleState)
        {
            // hide bars
            _player.HBar.gameObject.GetComponent<Image>().enabled = false;
            _player.EBar.gameObject.GetComponent<Image>().enabled = false;
            if (_player.CurrentCompanion)
            {
                _player.CurrentCompanion.HBar.gameObject.GetComponent<Image>().enabled = false;
                _player.CurrentCompanion.EBar.gameObject.GetComponent<Image>().enabled = false;
            }
        }
    }
}
