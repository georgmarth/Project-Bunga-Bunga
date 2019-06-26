using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskLocation : MonoBehaviour
{
    public PlayerTask Task;
    public PatronLocation[] PatronLocations;
    public Transform[] Locations;

    public bool Performing = false;

    public float UseLeft;

    private void Awake()
    {
        UseLeft = Task.MaxUseTime;
    }

    private void Update()
    {
        if (Performing)
        {
            UseLeft = Mathf.Clamp(UseLeft - Time.deltaTime, 0f, Task.MaxUseTime);
        }
    }

    public void FillUp()
    {
        UseLeft = Task.MaxUseTime;
    }
}

[System.Serializable]
public class PatronLocation
{
    public bool Reserved;
    public Patron Patron;
    public Transform Location;
    public TaskLocation TaskLocation;
}
