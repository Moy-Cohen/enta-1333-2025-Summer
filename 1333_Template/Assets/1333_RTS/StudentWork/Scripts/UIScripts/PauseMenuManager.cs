using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private AudioSource musicTrack;

    private bool isPaused;

    private void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (musicTrack != null)
        {
            if (isPaused)
            {
                musicTrack.Pause();
            }
            else
            {
                musicTrack.UnPause();
            }
        }
    }

    public void ReturnToGame() => TogglePause();
}
