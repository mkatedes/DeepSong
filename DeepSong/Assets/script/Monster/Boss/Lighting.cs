using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float lifetime = 0.05f;
    private CinemachineImpulseSource impulse;
    private bool hasHit = false;
    private DamageCanvaCall damageCanvaCall;

    private void Start()
    {
        damageCanvaCall = FindAnyObjectByType<DamageCanvaCall>();
        if(impulse != null)
            impulse?.GenerateImpulse();

        AudioManager.Instance.PlaySFX("ThunderStrike");
        StartCoroutine(NoDamage());
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (hasHit) return;

        if (other.gameObject.CompareTag("Player"))
        {
            EntityAttributes entityAttributes = other.GetComponent<EntityAttributes>();
            if (entityAttributes != null)
            {
               
                entityAttributes.TakeDamage(damage, transform.position);

                if (damageCanvaCall != null)
                {
                    damageCanvaCall.CallDamageCanva();
                }

                hasHit = true;

                Destroy(gameObject, 0.1f);
            }
        }
    }

    public void SetImpulseCamera(CinemachineImpulseSource cam)
    {
        impulse = cam;
    }
    private IEnumerator NoDamage()
    {
        yield return new WaitForSeconds(0.5f);
        hasHit = true;
    }
}