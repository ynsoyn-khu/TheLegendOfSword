using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public enum GameScene
    {
        Start,
        Intro,
        Level00,
        Level01,
        Level02,
        Level03,
        StageMenu,
        End,
        Setting
    }

    public void LoadScene(GameScene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadStageMenu()
    {
        SceneManager.LoadScene(GameScene.StageMenu.ToString());
    }

    public void LoadSetting()
    {
        SceneManager.LoadScene(GameScene.Setting.ToString());
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(GameScene.Level00.ToString());
    }

    public void GoToLevel01()
    {
        SceneManager.LoadScene(GameScene.Level01.ToString());
    }

    public void ThisStageRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
