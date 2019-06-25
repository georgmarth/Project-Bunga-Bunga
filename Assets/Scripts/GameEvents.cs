using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState { PLAYER_SELECT, RUNNING, PAUSED, GAMEOVER }

[CreateAssetMenu(menuName = "Game Assets/GameEvents")]
public class GameEvents : ScriptableObject
{
    public GameState GameState = GameState.PLAYER_SELECT;

    public Action<GameObject> PlayerJoined;
    public Action<GameObject> PlayerLeft;

    public Action GameStarted;
    public Action GameOver;
    public Action GameWon;
    public Action Pause;
    public Action Unpause;
    public Action<float> LevelTimer;

    public Action<Patron> PatronSpawned;
    public Action<Patron> PatronLeft;

    public Action<Patron> TaskFulfilled;

    public Action<int, string> OutfitListSwitched;

    public Func<Camera> GetCamera;

    public void OnEnable()
    {
        GameState = GameState.PLAYER_SELECT;
    }

    public void PlayerJoin(GameObject gameObject)
    {
        PlayerJoined?.Invoke(gameObject);
    }

    public void PlayerLeave(GameObject gameObject)
    {
        PlayerLeft?.Invoke(gameObject);
    }

    public void StartGame()
    {
        if (GameState == GameState.PLAYER_SELECT)
        {
            GameState = GameState.RUNNING;
            GameStarted?.Invoke();
        }
    }

    public void PauseGame()
    {
        if (GameState == GameState.RUNNING)
        {
            GameState = GameState.PAUSED;
            Pause?.Invoke();
        }
    }

    public void UnpauseGame()
    {
        if (GameState == GameState.PAUSED)
        {
            GameState = GameState.RUNNING;
            Unpause?.Invoke();
        }
    }
}