using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image slotImage;

    SO_Items itemData;

    public void Setup(SO_Items item)
    {
        itemData = item;

        if (slotImage != null && item.itemSprite != null)
        {
            slotImage.sprite = item.itemSprite;
            slotImage.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData != null && ItemsUiManager.Instance != null)
        {
            ItemsUiManager.Instance.ShowItem(itemData); // <- mostra l'oggetto (il SetAsLastSibling è già dentro ShowItem)
        }
    }
}