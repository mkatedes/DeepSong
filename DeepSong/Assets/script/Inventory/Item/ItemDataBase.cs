using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> items = new List<Item>();

    public Item GetItemByName(string itemName)
    {
        Item item = items.FirstOrDefault(i => i.itemName == itemName);

        if (item != null)
        {
            return Instantiate(item);
        }

        Debug.LogWarning($"Item '{itemName}' not found in database!");
        return null;
    }

    public List<Item> GetAllItems()
    {
        return items;
    }
}