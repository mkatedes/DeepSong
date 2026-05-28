using UnityEngine;

public class CombatColliderMonster : MonoBehaviour
{
    private EntityAttributes _ownerAttribute;

    private void Awake()
    {
        if (_ownerAttribute == null)
        {
            _ownerAttribute = GetComponentInParent<EntityAttributes>();
        }
}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")){

            _ownerAttribute.DealDamage(other.gameObject, other.ClosestPoint(transform.position));
            FindAnyObjectByType<DamageCanvaCall>().CallDamageCanva();
        }
    }
}
