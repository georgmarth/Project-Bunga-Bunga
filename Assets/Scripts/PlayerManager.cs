using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

public class PlayerManager : MonoBehaviour
{
    public PlayerInputManager PIM;
    public GameEvents GameEvents;

    private int playerCount = 0;

    private void Awake()
    {
        GameEvents.GameStarted += OnGameStart;
    }

    public void OnPlayerJoin(PlayerInput playerInput)
    {
        playerInput.GetComponent<PlayerController>().PlayerIndex = playerCount;
        playerCount++;
        GameEvents.PlayerJoin(playerInput.gameObject);
    }

    public void OnPlayerLeave(PlayerInput playerInput)
    {
        GameEvents.PlayerLeave(playerInput.gameObject);
    }

    public void OnGameStart()
    {
        PIM.DisableJoining();
    }
}
