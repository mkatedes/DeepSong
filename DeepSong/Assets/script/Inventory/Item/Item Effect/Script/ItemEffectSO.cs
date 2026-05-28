using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    public abstract void Execute();

    protected GameObject GetPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    protected PlayerMovement GetPlayerMovement()
    {
        GameObject player = GetPlayer();
        return player?.GetComponent<PlayerMovement>();
    }

    protected EntityAttributes GetPlayerAttributes()
    {
        GameObject player = GetPlayer();
        return player?.GetComponent<EntityAttributes>();
    }
}