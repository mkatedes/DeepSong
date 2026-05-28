using System.Collections.Generic;
using UnityEngine;

public class FootStepSystem : MonoBehaviour
{
    RaycastHit hit;
    private Transform rayStart;
    [SerializeField] private float range;
    [SerializeField] LayerMask layerMask;

    [System.Serializable]
    public class SurfaceSounds
    {
        public string surfaceTag;
        public List<AudioClip> soundNames = new List<AudioClip>();
    }

    private void Start()
    {
        rayStart = gameObject.transform;
    }

    [SerializeField] private List<SurfaceSounds> surfaceSoundsList = new List<SurfaceSounds>();
    [SerializeField] private List<AudioClip> defaultSounds = new List<AudioClip>();

    public void FootStep()
    {
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, range, layerMask))    
        {
            SurfaceSounds surface = surfaceSoundsList.Find(s => s.surfaceTag == hit.collider.tag);

            if (surface != null && surface.soundNames.Count > 0)
            {
                AudioClip randomSound = surface.soundNames[Random.Range(0, surface.soundNames.Count)];
                AudioManager.Instance.SFXSource.pitch = Random.Range(1f, 1.5f);
                AudioManager.Instance.SFXSource.volume = Random.Range(0.5f, 0.75f);
                AudioManager.Instance.SFXSource.PlayOneShot(randomSound);
            }
            else if (defaultSounds.Count > 0)
            {
                AudioClip randomSound = defaultSounds[Random.Range(0, defaultSounds.Count)];
                AudioManager.Instance.SFXSource.pitch = Random.Range(1f, 1.5f);
                AudioManager.Instance.SFXSource.volume = Random.Range(0.5f, 0.75f);
                AudioManager.Instance.SFXSource.PlayOneShot(randomSound);
            }
        }
    }
}
