using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks.Triggers;

public class SongEnable : MonoBehaviour
{
    [Header("Mechanics")]
    private float timeCounter = 0f;
    private bool isActivate = false;
    private int currentHitIndex = 0;
    private int successfulHitIndex = 0;

    [SerializeField] private float songDuration;
    [SerializeField] private float[] hitTimer;
    [SerializeField] private float hitMargin = 0.25f;

    [Header("Stele Visuals")]
    [SerializeField] private MeshRenderer steleMeshRenderer;
    [SerializeField] private float minEmissiveIntensity = 0.5f;
    [SerializeField] private float maxEmissiveIntensity = 5f;
    [SerializeField] private Color emissiveColor = Color.cyan;
    [SerializeField] private float glowRangeBeforeHit = 1f;
    [SerializeField] private float glowRangeAfterHit = 0.5f;
    [SerializeField] private PlayableDirector playable;
    [SerializeField] private GameObject doorCinematics;

    private Material steleMaterial;
    private CancellationTokenSource cts;

    void Start()
    {
        steleMaterial = steleMeshRenderer.material;
        steleMaterial.EnableKeyword("_EMISSION");
        SetEmissiveIntensity(minEmissiveIntensity);
        playable = doorCinematics.GetComponent<PlayableDirector>();
        doorCinematics.SetActive(false);
    }

    void Update()
    {
        if (isActivate)
        {
            timeCounter += Time.deltaTime;

            if (currentHitIndex < hitTimer.Length)
            {
                float hitTimerValue = hitTimer[currentHitIndex];
                if (timeCounter > hitTimerValue + glowRangeAfterHit)
                {
                    currentHitIndex++;
                }
            }

            if (timeCounter >= songDuration)
            {
                ResetSong();
            }
        }
    }

    void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    public void SetEmissiveIntensity(float intensity)
    {
        steleMaterial.SetColor("_EmissionColor", emissiveColor * intensity);
    }

    private float CalculateEmissiveFromProximity()
    {
        if (currentHitIndex >= hitTimer.Length)
            return minEmissiveIntensity;

        float nextHitTime = hitTimer[currentHitIndex];
        float timeDifference = timeCounter - nextHitTime;

        if (timeDifference < 0)
        {
            float distanceToHit = Mathf.Abs(timeDifference);

            if (distanceToHit <= glowRangeBeforeHit)
            {
                float proximityRatio = 1f - (distanceToHit / glowRangeBeforeHit);
                return Mathf.Lerp(minEmissiveIntensity, maxEmissiveIntensity, proximityRatio);
            }
        }
        else
        {
            float distanceAfterHit = timeDifference;

            if (distanceAfterHit <= glowRangeAfterHit)
            {
                float fadeRatio = distanceAfterHit / glowRangeAfterHit;
                return Mathf.Lerp(maxEmissiveIntensity, minEmissiveIntensity, fadeRatio);
            }
        }

        return minEmissiveIntensity;
    }

    private async UniTaskVoid UpdateEmissiveLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (isActivate)
            {
                float targetIntensity = CalculateEmissiveFromProximity();
                SetEmissiveIntensity(targetIntensity);
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    private async UniTask PlaySuccessAnimation(CancellationToken token)
    {
        for (int i = 0; i < 3; i++)
        {
            SetEmissiveIntensity(maxEmissiveIntensity * 2f);
            await UniTask.Delay(100, cancellationToken: token);
            SetEmissiveIntensity(minEmissiveIntensity);
            await UniTask.Delay(100, cancellationToken: token);
        }
    }

    private async UniTask PlayFailAnimation(CancellationToken token)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startIntensity = steleMaterial.GetColor("_EmissionColor").maxColorComponent;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(startIntensity, minEmissiveIntensity, t);
            SetEmissiveIntensity(intensity);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        SetEmissiveIntensity(minEmissiveIntensity);
    }

    private void ResetSong()
    {
        timeCounter = 0f;
        isActivate = false;
        currentHitIndex = 0;
        successfulHitIndex = 0;
        Debug.Log("Chanson réinitialisée");
        AudioManager.Instance.StopLoopingSFX("Melody");

        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        SetEmissiveIntensity(minEmissiveIntensity);
    }

    private async UniTaskVoid OnSongCompleted()
    {
        Debug.Log("BRAVO ! La stèle s'active !");

        doorCinematics.SetActive(true);
        try
        {
            await PlaySuccessAnimation(this.GetCancellationTokenOnDestroy());
        }
        catch (System.OperationCanceledException)
        {
        }

        ResetSong();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Sword")) return;

        if (!isActivate)
        {
            AudioManager.Instance.PlayLoopingSFX("Melody");
            isActivate = true;
            currentHitIndex = 0;
            successfulHitIndex = 0;
            Debug.Log("Chanson démarrée !");

            cts = new CancellationTokenSource();
            UpdateEmissiveLoop(cts.Token).Forget();

            return;
        }

        if (currentHitIndex < hitTimer.Length)
        {
            float targetTime = hitTimer[currentHitIndex];
            float timeDifference = Mathf.Abs(timeCounter - targetTime);

            if (timeDifference <= hitMargin)
            {
                Debug.Log($"HIT RÉUSSI ! Précision: {timeDifference:F3}s");
                successfulHitIndex++;

                if (successfulHitIndex >= hitTimer.Length)
                {
                    Debug.Log("CHANSON COMPLÉTÉE !");
                    OnSongCompleted().Forget();
                }
            }
            else
            {
                Debug.Log($"RATÉ ! Trop {(timeCounter < targetTime ? "tôt" : "tard")} de {timeDifference:F3}s");

                cts?.Cancel();
                PlayFailAnimation(this.GetCancellationTokenOnDestroy()).Forget();

                ResetSong();
            }
        }
    }
}