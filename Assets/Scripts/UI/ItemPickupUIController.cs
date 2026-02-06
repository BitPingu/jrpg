using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUIController : MonoBehaviour
{
    public static ItemPickupUIController Instance { get; private set; } // Singleton instance

    [SerializeField] private GameObject _popupPrefab;
    [SerializeField] private int _maxPopups = 3;
    [SerializeField] private float _popupDuration = 3f;

    private readonly Queue<GameObject> _activePopups = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Make sure only one instance
    }

    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(_popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
            itemImage.sprite = itemIcon;

        _activePopups.Enqueue(newPopup);
        if (_activePopups.Count > _maxPopups)
            Destroy(_activePopups.Dequeue());

        // fade out, destroy
        StartCoroutine(FadeOut(newPopup));
    }

    private IEnumerator FadeOut(GameObject popup)
    {
        yield return new WaitForSeconds(_popupDuration);
        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timePassed; // fade out image
            yield return null;
        }

        Destroy(popup);
    }
}
