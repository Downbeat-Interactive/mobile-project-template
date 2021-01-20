using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    static AudioSource audioSource;

    [SerializeField]
    AudioClip[] themes = null;
    private static AudioClip[] _themes;

    [SerializeField]
    float maxVolume = .5f;
    static float _maxVolume = .5f;

    static float _sfxVolume = 1;

    [SerializeField]
    float fadeTime = 0;

    bool isLooping = false;
    bool firstLoad = true;

    float timeSinceLastPlay = 0.0f;
    float timeBetweenSongsThreshold = 2.0f;
    private void Awake(){
        if (!AudioManager.Instance)
            AudioManager.Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();
        _themes = themes;
        _maxVolume = maxVolume;
        
        firstLoad = false;
    }

    internal static float GetMaxMusicVolume()
    {
        return _maxVolume;
    }

    internal static float GetSFXVolume()
    {
        return _sfxVolume;
    }

    internal static void SetSFXVolume(float v)
    {
        _sfxVolume = v;
    }
    internal static void SetMaxMusicVolume(float v)
    {
        _maxVolume = v;
        audioSource.volume = _maxVolume;
    }

    void PlayRandomThemeInternal(bool loop = false, float overrideFade = -1.0f) {
        AudioClip clip = _themes[UnityEngine.Random.Range(0, _themes.Length)];

        if(firstLoad)
            AnalyticsManager.LogDesign("firstSong_" + clip.name, _maxVolume);
        else
            AnalyticsManager.LogDesign("currentSong_" + clip.name, _maxVolume);

        PlayInternal(clip, overrideFade);
        isLooping = loop;
        //if (isLooping)
        //    StartCoroutine(Loop(clip.length - fadeTime / 2.0f));
    }

    IEnumerator Loop(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        PlayRandomThemeInternal(true);
    }

    void PlayInternal(AudioClip clip, float overrideFade = -1.0f) {
        if (audioSource.clip && audioSource.isPlaying)
        {
            //StartCoroutine(FadeBetweenEnum(clip));
            FadeToClip(clip, overrideFade);
        }
        else
            PlayImmediate(clip);
    }

    void PlayThemeInternal(int index, bool loop)
    {
        audioSource.loop = loop;
        if (audioSource.clip && audioSource.clip.Equals(themes[index]))
            return;

            if (audioSource.clip && audioSource.isPlaying)
        {
            //StartCoroutine(FadeBetweenEnum(clip));
            FadeToClip(themes[index]);
        }
        else
            PlayImmediate(themes[index]);
    }
    public static void PlayTheme(int index, bool loop = true)
    {
        Instance.PlayThemeInternal(index, loop);
    }


    private void Update(){
        if (!audioSource.isPlaying && isLooping) {
            audioSource.clip = null;
            PlayRandomThemeInternal(true, 0.0f);
        }
        
        //if (audioSource.isPlaying)
        //{
        //    timeSinceLastPlay = 0.0f;
        //}
        //else {
        //    if (isLooping) {
        //        timeSinceLastPlay += Time.deltaTime;
        //        if (timeSinceLastPlay > timeBetweenSongsThreshold) {
        //            PlayRandomThemeInternal();
        //        }
        //    }
        //}
    }

    private void FadeToClip(AudioClip clip, float overrideTime = -1)
    {
        float useTime = fadeTime;
        if (overrideTime >= 0)
            useTime = overrideTime;
        //Fade Out
        audioSource.DOFade(0, useTime / 2.0f).OnComplete(
            () => {
                //Set Track and fade in
                SetTrack(clip);
                audioSource.Play();
                audioSource.DOKill();
                audioSource.DOFade(GetMaxMusicVolume(), useTime / 2.0f); }
        );

       
    }

    void PlayImmediate(AudioClip clip) {
        SetTrack(clip);
        audioSource.Play();
    }


    public static void PlayRandomTheme(bool loop = false) {
        Instance.PlayRandomThemeInternal(loop);
    }
    static void SetTrack(AudioClip clipToSet) {
        
        audioSource.clip = clipToSet;
    }

    public static void Forwards()
    {
        audioSource.pitch = 1;
    }
    public static void Reverse() {
        //audioSource.timeSamples = audioSource.clip.samples - 1;
        audioSource.pitch = -1;
        //audioSource.Play();
    }

}
