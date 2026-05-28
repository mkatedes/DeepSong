using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private Item item;

    private RawImage itemImage;
    private TMP_Text amount;

    private void Awake()
    {
        itemImage = GetComponentInChildren<RawImage>();
        amount = GetComponentInChildren<TMP_Text>();
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateSlot();
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.UseItem();
            UpdateSlot();
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemImage.texture = null;
        itemImage.enabled = false;
        amount.text = "";
    }

    public void UpdateSlot()
    {
        if (item != null)
        {
            itemImage.texture = item.icon;
            itemImage.enabled = true;
            amount.text = item.amount > 1 ? item.amount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }
}
