using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public int PlayerSelectScene;
    public int LevelScene;

    public float LevelTime = 300;

    public Transform GameUI;

    public GameEvents GameEvents;

    private void OnEnable()
    {
        GameEvents.GameStarted += GameStarted;
        GameEvents.Pause += OnPause;
        GameEvents.Unpause += OnUnPause;
    }

    private void OnDisable()
    {
        GameEvents.GameStarted -= GameStarted;
        GameEvents.Pause -= OnPause;
        GameEvents.Unpause -= OnUnPause;
    }

    private void Update()
    {
        if (GameEvents.GameState == GameState.RUNNING)
        {
            LevelTime -= Time.deltaTime;
            GameEvents.LevelTimer?.Invoke(LevelTime);
        }
    }

    public void OnPause()
    {
        Time.timeScale = 0f;
        GameUI.gameObject.SetActive(false);
    }

    public void OnUnPause()
    {
        GameUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    private void Start()
    {
        SceneManager.LoadScene(PlayerSelectScene, LoadSceneMode.Additive);
    }

    private void GameStarted()
    {
        SceneManager.UnloadSceneAsync(PlayerSelectScene);
        GameUI.gameObject.SetActive(true);
    }
}
