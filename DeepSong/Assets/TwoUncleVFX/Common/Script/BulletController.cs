using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace VFXTools
{
    public enum TowardType
    {
        Forward,
        Right
    }

    public class BulletController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private TowardType towardType = TowardType.Forward;
        [SerializeField] private float maxDistance = 100f;

        [Header("Lifetime")]
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float delayBeforeMoving = 0f;

        [Header("Damage")]
        [SerializeField] private int damage = 10;

        private VisualEffect[] vfxs;
        private TrailRenderer[] trails;
        private float traveledDistance = 0f;
        private bool isActive = false;

        private void Start()
        {
            vfxs = GetComponentsInChildren<VisualEffect>();
            trails = GetComponentsInChildren<TrailRenderer>();

            StartCoroutine(ProjectileLifecycle());
        }

        private IEnumerator ProjectileLifecycle()
        {
            // Délai initial si nécessaire
            if (delayBeforeMoving > 0)
            {
                SetVFXEnabled(false);
                yield return new WaitForSeconds(delayBeforeMoving);
                SetVFXEnabled(true);
            }

            isActive = true;

            float elapsed = 0f;
            while (elapsed < lifetime && traveledDistance < maxDistance)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private void Update()
        {
            if (!isActive) return;

            Vector3 movement = towardType == TowardType.Forward
                ? transform.forward
                : transform.right;

            float distance = movementSpeed * Time.deltaTime;
            transform.position += movement * distance;
            traveledDistance += distance;
        }

        private void SetVFXEnabled(bool enabled)
        {
            foreach (var vfx in vfxs)
            {
                if (vfx != null) vfx.enabled = enabled;
            }
            foreach (var trail in trails)
            {
                if (trail != null) trail.enabled = enabled;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 direction = towardType == TowardType.Forward ? transform.forward : transform.right;
            Gizmos.DrawRay(transform.position, direction * 2f);
        }
    }
}