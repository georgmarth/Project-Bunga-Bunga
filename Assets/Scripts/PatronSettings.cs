using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Assets/PatronSettings")]
public class PatronSettings : ScriptableObject
{
    public int DesiredTasks = 3;
    public int IdleLocations = 3;
    public Vector2 IdleTime = new Vector2(5f, 10f);
    public Vector2 IdleWalkTime = new Vector2(5f, 10f);
    public float Patience = 20f;
}
