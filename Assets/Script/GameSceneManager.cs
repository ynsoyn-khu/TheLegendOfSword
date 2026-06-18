using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Start Scene Buttons")]
    [SerializeField] Button _gameStartButton;
    [SerializeField] Button _quitButton;
    [SerializeField] Button _settingButton;

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

    private void Start()
    {
        if (_gameStartButton != null) _gameStartButton.onClick.AddListener(LoadNextScene);
        if (_quitButton != null) _quitButton.onClick.AddListener(QuitGame);
        if (_settingButton != null) _settingButton.onClick.AddListener(LoadSetting);
    }

    public enum GameScene
    {
        Start,
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
