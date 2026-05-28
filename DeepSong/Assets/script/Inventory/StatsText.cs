using UnityEngine;
using TMPro;

public class StatsText : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private EntityAttributes entityAttributes;

    void FixedUpdate()
    {
        armorText.text = "Armor: " + entityAttributes.Armor;
        damageText.text = "Damage: " + entityAttributes.AttackDamage;
    }

}
