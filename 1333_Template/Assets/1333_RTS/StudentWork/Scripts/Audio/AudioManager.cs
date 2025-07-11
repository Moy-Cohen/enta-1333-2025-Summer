using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct NamedAudioClip
{
    public string Name;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Setup")]
    public AudioMixer AudioMixer;
    public AudioMixerGroup MusicGroup;
    public AudioMixerGroup SfxGroup;

    [Header("Sources")]
    public AudioSource MusicSource;
    public GameObject SfxSourcePrefab;

    [Header("Audio Clips")]
    [SerializeField] private List<NamedAudioClip> namedClips = new();
    private Dictionary<string, AudioClip> sfxDictionary = new();

    private List<AudioSource> sfxPool = new();
    private int poolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var entry in namedClips)
        {
            if (!sfxDictionary.ContainsKey(entry.Name))
            {
                sfxDictionary.Add(entry.Name, entry.Clip);
            }
        }
    }


    private void InitializeSFXPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(SfxSourcePrefab, transform);
            AudioSource src = go.GetComponent<AudioSource>();
            src.outputAudioMixerGroup = SfxGroup;
            src.playOnAwake = false;
            sfxPool.Add(src);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.outputAudioMixerGroup = MusicGroup;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        AudioSource src = GetAvailableSFXSource();
        if (src !=  null)
        {
            src.clip = clip;
            src.volume = volume;
            src.Play();
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach(var src in sfxPool)
        {
            if(!src.isPlaying) return src;
        }

        // Expand pool if needed
        GameObject go = Instantiate(SfxSourcePrefab,transform);
        AudioSource newSrc = go.GetComponent<AudioSource>();
        newSrc.outputAudioMixerGroup= SfxGroup;
        sfxPool.Add(newSrc);
        return newSrc;
    }

    public void SetMusicVolume(float volume)
    {
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void PlaySFX(string clipName, float volume = 1f)
    {
        if(sfxDictionary.TryGetValue(clipName, out var clip))
        {
            PlaySFX(clip, volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{clipName}' not found");
        }
    }

}
