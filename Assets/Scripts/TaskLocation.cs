﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskLocation : MonoBehaviour
{
    public PlayerTask Task;
    public PatronLocation[] PatronLocations;
    public Transform[] Locations;
}

[System.Serializable]
public class PatronLocation
{
    public bool Reserved;
    public Patron Patron;
    public Transform Location;
}