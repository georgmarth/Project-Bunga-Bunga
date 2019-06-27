using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TaskLocation : MonoBehaviour
{
    public PlayerTask Task;
    public PatronLocation[] PatronLocations;
    public Transform[] Locations;

    public int Performing = 0;

    public float UseLeft;

    public Action<float> UsePercentageLeft;

    public Action<float> UpdateSynchronizedTime;

    private float _normalizedTime;

    public string GetAnimationString()
    {
        return Task.AnimationStrings[Performing];
    }

    private void Awake()
    {
        _normalizedTime = 0f;
        UseLeft = Task.MaxUseTime;
        UsePercentageLeft?.Invoke(UseLeft / Task.MaxUseTime);
    }

    private void Update()
    {
        if (Performing >= Task.RequiredPlayerCount && Task.RequiresPot)
        {
            UseLeft = Mathf.Clamp(UseLeft - Time.deltaTime, 0f, Task.MaxUseTime);
            UsePercentageLeft?.Invoke(UseLeft / Task.MaxUseTime);
        }
        if (Task.AnimationTime > 0f)
        {
            _normalizedTime += Time.deltaTime / Task.AnimationTime;
            _normalizedTime -= Mathf.Floor(_normalizedTime);
            UpdateSynchronizedTime?.Invoke(_normalizedTime);
        }
    }

    public void FillUp()
    {
        UseLeft = Task.MaxUseTime;
        UsePercentageLeft?.Invoke(UseLeft / Task.MaxUseTime);
    }
}

[System.Serializable]
public class PatronLocation
{
    public bool Reserved;
    public Patron Patron;
    public Transform Location;
    public Transform ExitLocation;
    public TaskLocation TaskLocation;
}
