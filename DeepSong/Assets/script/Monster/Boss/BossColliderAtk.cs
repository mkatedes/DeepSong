using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class BossColliderAtk : MonoBehaviour
{
    [SerializeField] private EntityAttributes _ownerAttribute;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float upwardForce = 1f;
    private void Reset()
    {
        _ownerAttribute = GetComponentInParent<EntityAttributes>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _ownerAttribute.DealDamage(other.gameObject, other.ClosestPoint(transform.position));
            FindAnyObjectByType<DamageCanvaCall>().CallDamageCanva();

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                ApplyKnockbackAsync(rb, other.transform).Forget();
            }
        }
    }

    private async UniTaskVoid ApplyKnockbackAsync(Rigidbody rb, Transform targetTransform)
    {
        Vector3 direction = -targetTransform.forward;
        Vector3 force = direction * knockbackForce + Vector3.up * upwardForce;
        rb.AddForce(force, ForceMode.Impulse);

        await UniTask.WaitForSeconds(1f);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
