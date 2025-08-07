using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject howToPlayPanel;


    [SerializeField] private Button continueButton;

    private void Start()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveManager.SaveExists();
        }
    }

    public void PlayGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        SaveManager.DeleteSave();
        StartCoroutine(LoadGameAsync("SampleScene"));
    }

    public void ContinueGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        if (!SaveManager.SaveExists()) return;
        SaveManager.LoadGame();
        StartCoroutine(LoadGameAsync(gameplaySceneName));
    }

    private IEnumerator LoadGameAsync(string sceneName)
    {
        
        if (loadingPanel != null) loadingPanel.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            }
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }


    }

    public void Instructions()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        howToPlayPanel.SetActive(true);
    }

    public void ExitInstructions()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        howToPlayPanel.SetActive(false);
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        Application.Quit();
    }
}
