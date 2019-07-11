using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public int PlayerSelectScene;
    public int LevelScene;

    public float LevelTime = 300;

    public Transform GameUI;
    public Transform PauseScreen;
    public Transform GameOverScreen;
    public TextMeshProUGUI ScoreText;

    public GameEvents GameEvents;

    private void OnEnable()
    {
        GameEvents.GameStarted += GameStarted;
        GameEvents.Pause += OnPause;
        GameEvents.Unpause += OnUnPause;
        GameEvents.GameOver += OnGameOver;
        GameEvents.LevelTimer += OnLevelTimer;
        GameEvents.Restart += OnRestart;
    }

    private void OnDisable()
    {
        GameEvents.GameStarted -= GameStarted;
        GameEvents.Pause -= OnPause;
        GameEvents.Unpause -= OnUnPause;
        GameEvents.GameOver -= OnGameOver;
        GameEvents.LevelTimer -= OnLevelTimer;
        GameEvents.Restart -= OnRestart;
    }

    private void Update()
    {
        if (GameEvents.GameState == GameState.RUNNING)
        {
            LevelTime -= Time.deltaTime;
            GameEvents.LevelTimer?.Invoke(LevelTime);
        }
    }

    private void OnLevelTimer(float time)
    {
        if (time <= 0f)
        {
            GameEvents.EndGame();
        }
    }

    public void OnPause()
    {
        GameUI.gameObject.SetActive(false);
        PauseScreen.gameObject.SetActive(true);
        GameOverScreen.gameObject.SetActive(false);
        Time.timeScale = 0f;
    }

    public void OnUnPause()
    {
        GameUI.gameObject.SetActive(true);
        PauseScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnGameOver()
    {
        GameUI.gameObject.SetActive(false);
        PauseScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
        ScoreText.SetText(GameEvents.Money.ToString());
        Time.timeScale = 0f;
    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        GameEvents.SetDefault();
        //SceneManager.UnloadSceneAsync(PlayerSelectScene);
        //SceneManager.UnloadSceneAsync(LevelScene).completed += (_ => AllUnloaded());
        SceneManager.LoadScene(LevelScene);
    }

    private void AllUnloaded()
    {
        SceneManager.LoadScene(LevelScene);
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
