using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Inventory/Effects/Heal")]
public class HealEffect : ItemEffectSO
{
    [SerializeField] private int healAmount = 20;

    public override void Execute()
    {
        EntityAttributes playerAttributes = GetPlayerAttributes();
        if (playerAttributes != null)
        {
            AudioManager.Instance.PlaySFX("Heal", false);
            playerAttributes.Heal(healAmount);
            FindAnyObjectByType<HealthHeartBar>().DrawHeart();
        }
    }
}