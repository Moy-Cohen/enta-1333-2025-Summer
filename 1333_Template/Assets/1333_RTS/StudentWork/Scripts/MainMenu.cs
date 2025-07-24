using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private UnityEngine.UI.Slider progressBar;

    public void PlayGame()
    {
        StartCoroutine(LoadGameAsync("SampleScene"));
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

    public void ContinueGame()
    {
        //Add Logic for Game save file
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
