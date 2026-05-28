using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DamageCanvaCall : MonoBehaviour
{
    [SerializeField] private Canvas hitCanvas;

    public float intensity = 0;
    private Color colorAlpha = Color.red;
    [SerializeField] private RawImage hitImage;

    void Start()
    {
        hitCanvas.enabled = false;
    }
    private IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        hitCanvas.enabled = true;
        colorAlpha.a = 0.4f;
        hitImage.color = colorAlpha;

        yield return new WaitForSeconds(0.01f);

        while (intensity > 0.0f)
        {
            intensity -= Time.deltaTime;
            if (intensity < 0) intensity = 0;

            colorAlpha.a = intensity;
            hitImage.color = colorAlpha;
            yield return new WaitForSeconds(0.01f);
        }

        hitCanvas.enabled = false;
        yield break;
    }

    public void CallDamageCanva()
    {
        StartCoroutine(TakeDamageEffect());
    }

}
