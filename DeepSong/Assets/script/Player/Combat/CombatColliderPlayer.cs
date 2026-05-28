using UnityEngine;

public class CombatColliderPlayer : MonoBehaviour
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
        if (other.gameObject.CompareTag("Enemy")){

            _ownerAttribute.DealDamage(other.gameObject, other.ClosestPoint(transform.position));
            Debug.Log(other.ClosestPoint(transform.position).ToString());
        }
    }
}
