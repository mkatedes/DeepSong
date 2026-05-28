using UnityEngine;
using TMPro;


public class UpdateCoin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    void Update()
    {
        coinText.text = Inventory.Instance.Coins.ToString();
    }
}
