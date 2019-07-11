using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState { PLAYER_SELECT, RUNNING, PAUSED, GAMEOVER }

[CreateAssetMenu(menuName = "Game Assets/GameEvents")]
public class GameEvents : ScriptableObject
{
    public GameState GameState = GameState.PLAYER_SELECT;

    public int Money { get; private set; }

    public Action<GameObject> PlayerJoined;
    public Action<GameObject> PlayerLeft;

    public Action GameStarted;
    public Action GameOver;
    public Action GameWon;
    public Action Pause;
    public Action Unpause;
    public Action Restart;
    public Action<float> LevelTimer;

    public Action<Patron> PatronSpawned;
    public Action<Patron, bool> PatronLeft;

    public Action<Patron> TaskFulfilled;

    public Action<int> MoneyChanged;

    public Action<int, string> OutfitListSwitched;

    public Func<Camera> GetCamera;

    public void OnEnable()
    {
        SetDefault();
    }

    public void SetDefault()
    {
        Money = 0;
        GameState = GameState.PLAYER_SELECT;

        PlayerJoined = null;
        PlayerLeft = null;

        GameStarted = null;
        GameOver = null;
        GameWon = null;
        Pause = null;
        Unpause = null;
        Restart = null;
        LevelTimer = null;

        PatronSpawned = null;
        PatronLeft = null;
        TaskFulfilled = null;
        MoneyChanged = null;
        OutfitListSwitched = null;
        GetCamera = null;
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

    public void EndGame()
    {
        GameState = GameState.GAMEOVER;
        GameOver?.Invoke();
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        MoneyChanged?.Invoke(Money);
    }
}