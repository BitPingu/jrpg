using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; } // Singleton instance

    private Dictionary<string, ItemBase> _items = new Dictionary<string, ItemBase>();

    [SerializeField] private Image _itemSlot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    public void AddItem(ItemBase item)
    {
        _items.Add(item.itemName, item);
        _itemSlot.sprite = item.icon;
        _itemSlot.enabled = true;

        if (ItemPickupUIController.Instance != null)
            ItemPickupUIController.Instance.ShowItemPickup(item.itemName, item.icon);
    }

    public ItemBase GetItem(string name)
    {
        ItemBase item = null;
        if (_items.ContainsKey(name))
        {
            item = _items[name];
            _items.Remove(name);
            _itemSlot.sprite = null;
            _itemSlot.enabled = false;
        }
        return item;
    }
}
