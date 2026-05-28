using Tiny;
using UnityEngine;

[CreateAssetMenu(fileName = "ProtectionGemEffect", menuName = "Inventory/Effects/ProtectionGem")]
public class ProtectionGemEffect : ItemEffectSO
{
    public override void Execute()
    {
        EntityAttributes playerAttributes = GetPlayerAttributes();
        if (playerAttributes != null)
        {
            playerAttributes.Armor += 1;
            if (playerAttributes.Armor > 3)
            {
                playerAttributes.Armor = 3;
            }
            AudioManager.Instance.PlaySFX("ArmorUpgrade");    
        }
    }
}
