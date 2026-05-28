using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LightingSpawn : MonoBehaviour
{
    private GameObject lightingStrikePrefab;
    [SerializeField] private float maxScale = 3f;
    [SerializeField] private float growSpeed = 3f;
    [SerializeField] private float delayBeforeStrike = 0.2f;

    private CinemachineImpulseSource impulseSource;

    public void SetImpulseSource(CinemachineImpulseSource source)
    {
        if(source != null)
            impulseSource = source;
    }
    public void SetLightingStrikePrefab(GameObject prefab)
    {
        lightingStrikePrefab = prefab;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(GrowAndStrike());
    }

    private IEnumerator GrowAndStrike()
    {
        while (transform.localScale.x < maxScale)
        {
            float growAmount = growSpeed * Time.deltaTime;
            Vector3 newScale = transform.localScale + new Vector3(growAmount, growAmount, growAmount);
            newScale = Vector3.Min(newScale, Vector3.one * maxScale);
            transform.localScale = newScale;
            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeStrike);

        if (lightingStrikePrefab != null)
        {
            GameObject instance = Instantiate(lightingStrikePrefab, transform.position, Quaternion.identity);
            Lighting lighting = instance.GetComponent<Lighting>();
            if (lighting != null)
            {
                lighting.SetImpulseCamera(impulseSource); 
            }
        }
        else
        {
            Debug.LogWarning("lightingStrikePrefab n'a pas été assigné !");
        }

        Destroy(gameObject);
    }
}
