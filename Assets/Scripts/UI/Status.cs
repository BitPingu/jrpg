using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class Status : MonoBehaviour
{
    public static Status Instance { get; private set; } // Singleton instance

    private Player _player;
    public bool IsOn { get; set; }
    [SerializeField] private GameObject _inventoryBox;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance

        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_player.Input.Q && _player.StateMachine.CurrentState == _player.IdleState && !DialogueController.Instance.IsDialogueActive)
        {
            _player.StateMachine.End(); // disable movement
            IsOn = true;
        }
        else if (_player.Input.Q && IsOn)
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

            // inventory
            _inventoryBox.SetActive(true);

            if (_player.Input.E)
            {
                // TODO: need to make this more dynamic
                ItemBase item = InventoryController.Instance.GetItem("Potion");
                if (item is Consumable)
                {
                    Debug.Log(_player.name + " used a " + item.name + ".");
                    _player.Heal(((Consumable)item).amount);
                }
                else
                {
                    Debug.Log("There are no consumable items.");
                }
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

            // inventory
            _inventoryBox.SetActive(false);
        }
    }
}
