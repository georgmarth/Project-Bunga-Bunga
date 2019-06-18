using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class GameEvents : ScriptableObject
{
    public Action<GameObject> PlayerJoined;
    public Action<GameObject> PlayerLeft;

    public void PlayerJoin(GameObject gameObject)
    {
        PlayerJoined?.Invoke(gameObject);
    }

    public void PlayerLeave(GameObject gameObject)
    {
        PlayerLeft?.Invoke(gameObject);
    }
}