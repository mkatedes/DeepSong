using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    private Dictionary<string, AudioSource> musicSources = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> sfxSources = new Dictionary<string, AudioSource>();
    [SerializeField]private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private Sound[] sounds;

    public AudioSource SFXSource
    {
        get { return sfxSource; }
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySFX(string name, bool randomPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"[AudioManager] Aucun son trouvé avec le nom : {name}");
            return;
        }
        if (randomPitch)
        {
            sfxSource.pitch = UnityEngine.Random.Range(0.8f, 1.05f);
        }
        else
        {
            sfxSource.pitch = 1f;
        }
         sfxSource.PlayOneShot(s.clip, s.volume);
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"[AudioManager] Aucun son trouvé avec le nom : {name}");
            return;
        }
        sfxSource.pitch = UnityEngine.Random.Range(0.8f, 1.05f);
        sfxSource.PlayOneShot(s.clip, s.volume);
    }
    public void PlayMusic(string name, bool additive = false)
    {
        Debug.Log($"[AudioManager] PlayMusic appelé avec : {name}");

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"[AudioManager] Aucune musique trouvée : {name}");
            return;
        }

        Debug.Log($"[AudioManager] Son trouvé : {s.name}, Clip : {s.clip?.name ?? "NULL"}");

        if (!additive)
        {
            StopAllMusic();
        }

        if (!musicSources.ContainsKey(name))
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.loop = true;
            musicSources[name] = newSource;
            Debug.Log($"[AudioManager] Nouvelle AudioSource créée pour : {name}");
        }

        AudioSource source = musicSources[name];
        source.clip = s.clip;
        source.volume = s.volume;
        source.Play();

        Debug.Log($"[AudioManager] Play() appelé. IsPlaying : {source.isPlaying}, Volume : {source.volume}");
    }

    public void StopAllMusic()
    {
        foreach (var source in musicSources.Values)
        {
            source.Stop();
        }
    }


    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayLoopingSFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;

        if (!sfxSources.ContainsKey(name))
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.loop = true;
            sfxSources[name] = newSource;
        }

        AudioSource source = sfxSources[name];
        source.clip = s.clip;
        source.volume = s.volume;
        source.Play();
    }

    public void StopLoopingSFX(string name)
    {
        if (sfxSources.ContainsKey(name))
        {
            sfxSources[name].Stop();
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 5f)] public float volume = 1f;
}
