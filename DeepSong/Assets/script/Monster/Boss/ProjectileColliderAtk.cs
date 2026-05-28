using UnityEngine;

public class ProjectileColliderAtk : MonoBehaviour
{
    private EntityAttributes _ownerAttribute;

    public EntityAttributes OwnerAttribute
    {
        get => _ownerAttribute;
        set => _ownerAttribute = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _ownerAttribute.DealDamage(other.gameObject, other.ClosestPoint(transform.position));
            FindAnyObjectByType<DamageCanvaCall>().CallDamageCanva();
        }
    }
}
