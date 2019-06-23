using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Assets/PlayerTask")]
public class PlayerTask : ScriptableObject
{
    public string AnimationString;
    public int RequiredPlayerCount;
    public float RequiredTime;
}