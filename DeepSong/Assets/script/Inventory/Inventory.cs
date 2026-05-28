using UnityEngine;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] private ItemDatabase itemDatabase;
    private List<Item> items = new List<Item>();

    [SerializeField]private List<GameObject> slots = new List<GameObject>();
    private List<Slot> slotComponents = new List<Slot>();

    private int coins = 0;

    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value;
            UpdateInventory();
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (GameObject slotObject in slots)
        {
            Slot slot = slotObject.GetComponent<Slot>();
            if (slot != null)
            {
                slotComponents.Add(slot);
            }
        }
    }


    public void GiveRandomItem()
    {
        List<Item> allItem = itemDatabase.GetAllItems();
        Item randomItem = allItem[Random.Range(0, allItem.Count)];
        randomItem.amount = 1;
        AddItem(randomItem);
    }
    public void AddItemByName(string itemName, int amount = 1)
    {
        Item itemTemplate = itemDatabase.GetItemByName(itemName);

        if (itemTemplate != null)
        {
            itemTemplate.amount = amount;
            AddItem(itemTemplate);
        }
    }

    public void AddItem(Item item)
    {
        Item existingItem = items.Find(i => i.name == item.name);

        if (existingItem != null)
        {
            existingItem.amount += item.amount;
        }
        else
        {
            items.Add(item);
        }

        UpdateInventory();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);


        UpdateInventory();
    }

    public void RemoveItem(Item item, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == item.name)
            {
                items[i].amount -= amount;
                if (items[i].amount <= 0)
                {
                    items.RemoveAt(i);
                }
                break;
            }
        }
        UpdateInventory();
    }

    public void ClearInventory()
    {
        items.Clear();
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        foreach (Slot slot in slotComponents)
        {
            slot.ClearSlot();
        }

        
        for (int i = 0; i < items.Count && i < slotComponents.Count; i++)
        {
            slotComponents[i].SetItem(items[i]);
        }

    }
}
