using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    public GameEvents GameEvents;
    public AudioClip MainMusic;
    private AudioSource _audio;

    void OnEnable()
    {
        _audio = GetComponent<AudioSource>();
        GameEvents.GameStarted += OnGameStart;
    }

    private void OnDisable()
    {
        GameEvents.GameStarted -= OnGameStart;
    }

    private void OnGameStart()
    {
        _audio.clip = MainMusic;
        _audio.Play();
    }
}
