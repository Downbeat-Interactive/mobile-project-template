using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFXSourceVolume : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().volume = AudioManager.GetSFXVolume();
    }

    
}
