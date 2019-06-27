using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX2 : MonoBehaviour
{
    public GameEvents GameEvents;
    public AudioSource AudioSource;
    public AudioClip TaskFulfilledClip;

    private void OnEnable()
    {
        GameEvents.TaskFulfilled += OnTaskFulfilled;
    }

    private void OnDisable()
    {
        GameEvents.TaskFulfilled -= OnTaskFulfilled;
    }

    private void OnTaskFulfilled(Patron patron)
    {
        AudioSource.clip = TaskFulfilledClip;
        AudioSource.Play();
    }
}
