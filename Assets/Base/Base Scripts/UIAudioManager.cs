using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [SerializeField]
    AudioClip[] clickSounds = null;

    AudioSource source;

    private void Awake()
    {
        if (Instance)
            Destroy(this.gameObject);
        else
            Instance = this;
        source = GetComponent<AudioSource>();
    }

    private void InternalPlayClickSound(AudioClip clipOverride = null) {
        if (clipOverride == null)
            source.PlayOneShot(clickSounds[Random.Range(0, clickSounds.Length)]);
        else
            source.PlayOneShot(clipOverride);
    }

    public static void PlayClickSound(AudioClip clipOverride = null) {
        Instance.InternalPlayClickSound(clipOverride);
    }

}
