using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource AudioSource;

    public void PlaySound(AudioClip clip)
    {
        if (!AudioSource.isPlaying)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
        }
    }
}
