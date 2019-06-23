using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game Assets/GameEvents")]
public class GameEvents : ScriptableObject
{
    public Action<GameObject> PlayerJoined;
    public Action<GameObject> PlayerLeft;

    public Action GameStarted;
    public Action GameOver;
    public Action GameWon;
    public Action Pause;
    public Action Unpause;
    public Action<float> LevelTimer;

    public Action<Patron> TaskFulfilled;

    public void PlayerJoin(GameObject gameObject)
    {
        PlayerJoined?.Invoke(gameObject);
    }

    public void PlayerLeave(GameObject gameObject)
    {
        PlayerLeft?.Invoke(gameObject);
    }    
}