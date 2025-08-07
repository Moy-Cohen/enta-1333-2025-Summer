using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenuScene";

    private GameContext ctx;

    private void Awake()
    {
        ctx = new GameContext
        {
            WaveManager = FindObjectOfType<EnemyWaveManager>(),
            ResourceManager = FindObjectOfType<ResourceManager>(),
            ScoreManager = FindObjectOfType<ScoreManager>(),
            LaneManager = FindObjectOfType<LaneManager>(),
            AudioUI = FindObjectOfType<AudioSettingUI>()
        };
    }

    public void OnSaveAndQuit()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        SaveManager.SaveGame(ctx);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
