using Unity.VisualScripting;
using UnityEngine;

public class ItemBuy : MonoBehaviour
{
    [SerializeField]
    private int prize;
    [SerializeField]
    private string item_name;

    public void Buy()
    {
        if (Inventory.Instance.Coins > prize)
        {
            Inventory.Instance.Coins -= prize;
            Inventory.Instance.AddItemByName(item_name);
        }
        else
            Debug.Log("no money");

    }
}
