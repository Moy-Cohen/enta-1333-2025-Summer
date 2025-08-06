using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] public Slider MusicSlider;
    [SerializeField] public Slider SFXSlider;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    private void Awake()
    {
        if (mixer.GetFloat(MUSIC_PARAM, out float mDb))
        {
            MusicSlider.value = DbToLinear(mDb);
        }
        if(mixer.GetFloat(SFX_PARAM, out float sDb))
        {
            SFXSlider.value = DbToLinear(sDb);
        }

        MusicSlider.onValueChanged.AddListener(val => mixer.SetFloat(MUSIC_PARAM, LinearToDb(val)));

        SFXSlider.onValueChanged.AddListener(val => mixer.SetFloat(SFX_PARAM, LinearToDb(val)));

        MusicSlider.onValueChanged.AddListener(val =>
        {
            Debug.Log($"Music slider changed to {val}");   // <-- this line should print
            mixer.SetFloat(MUSIC_PARAM, LinearToDb(val));
        });

        SFXSlider.onValueChanged.AddListener(val =>
        {
            Debug.Log($"SFX slider changed to {val}");     // <-- and this one
            mixer.SetFloat(SFX_PARAM, LinearToDb(val));
        });

    }

    static float LinearToDb(float v) => Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
    static float DbToLinear(float db) => Mathf.Pow(10f, db / 20f);



}
