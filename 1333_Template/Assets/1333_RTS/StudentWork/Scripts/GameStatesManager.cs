using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatesManager : MonoBehaviour
{
    public static GameStatesManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject gameOverPanel;

    

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
    }


    private void Start()
    {
        StartCoroutine(ShowLoadingBuffer());
    }

    private IEnumerator ShowLoadingBuffer()
    {
        SetState(GameState.Loading);
        if (loadingPanel != null) loadingPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (loadingPanel != null) loadingPanel.SetActive(false);
        SetState(GameState.Playing);

        
    }
        


    public void SetState(GameState newState)
    {
        CurrentState = newState;
        UpdateUI();
    }

    private void UpdateUI()
    {
       if(loadingPanel != null) loadingPanel.SetActive(CurrentState == GameState.Loading);
       if(gameOverPanel != null) gameOverPanel.SetActive(CurrentState == GameState.GameOver);
    }

    public void ShowGameOver()
    {
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
