using Tiny;
using UnityEngine;

[CreateAssetMenu(fileName = "ForgegStoneEffect", menuName = "Inventory/Effects/ForgegStone")]
public class ForgedStoneEffect : ItemEffectSO
{
    [SerializeField] private Material trailLvl1;
    [SerializeField] private Material trailLvl2;
    [SerializeField] private Material trailLvl3;
    public override void Execute()
    {
        EntityAttributes playerAttributes = GetPlayerAttributes();
        if (playerAttributes != null)
        {
            playerAttributes.BaseDamage += 1;
            if (playerAttributes.BaseDamage > 3)
            {
                playerAttributes.BaseDamage = 3;
            }
            AudioManager.Instance.PlaySFX("SwordUpgrade");
            Trail trail = FindAnyObjectByType<Trail>();
            if (trail != null)
            {
                switch (playerAttributes.BaseDamage)
                {
                    case 1:
                        trail.Material = trailLvl1;
                        break;
                    case 2:
                        trail.Material = trailLvl2;
                        break;
                    case 3:
                        trail.Material = trailLvl3;
                        break;
                }
            }
        }
    }
}
