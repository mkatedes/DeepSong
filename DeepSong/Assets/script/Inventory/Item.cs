using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{
    public Texture2D icon;
    public string itemName;
    public ItemType itemType;
    public int amount;

    [Header("Effects")]
    public ItemEffectSO[] effects;

    public void UseItem()
    {
        foreach (ItemEffectSO effect in effects)
        {
            if (effect != null)
            {
                effect.Execute();
            }
        }

        Inventory.Instance.RemoveItem(this, 1);
    }
}
