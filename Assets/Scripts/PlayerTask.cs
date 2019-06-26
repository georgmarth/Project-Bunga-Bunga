using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Assets/PlayerTask")]
public class PlayerTask : ScriptableObject
{
    public string[] AnimationStrings;
    public int RequiredPlayerCount;
    public float RequiredTime;

    public bool RequiresPot;
    public string PotString;

    public string PatronString;

    public float MaxUseTime;
}