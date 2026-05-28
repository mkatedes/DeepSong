using Cysharp.Threading.Tasks;
using Tiny;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "HasteEffect", menuName = "Inventory/Effects/HasteEffect")]
public class HasteEffect : ItemEffectSO
{
    public override void  Execute()
    {
        overrideExecute();
    }

    public async UniTaskVoid overrideExecute() 
    {
        PlayerMovement playerMouv = GetPlayerMovement();
        if (playerMouv != null)
        {
            playerMouv.Speed++;
            AudioManager.Instance.PlaySFX("ArmorUpgrade");
            await UniTask.WaitForSeconds(10);
            playerMouv.Speed--;
        }
    }

}

