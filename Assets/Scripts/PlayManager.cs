using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public int PlayerSelectScene;
    public int LevelScene;

    public GameEvents GameEvents;

    private void Awake()
    {
        GameEvents.GameStarted += UnloadPlayerSelectScene;
    }

    private void Start()
    {
        SceneManager.LoadScene(PlayerSelectScene, LoadSceneMode.Additive);
    }

    private void UnloadPlayerSelectScene()
    {
        SceneManager.UnloadSceneAsync(PlayerSelectScene);
    }
}
