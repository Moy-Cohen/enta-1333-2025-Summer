using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHUDDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceCounter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI waveCounter;
    [SerializeField] private TextMeshProUGUI timerCounter;

    [SerializeField] private EnemyWaveManager waveManager;
    
    void Update()
    {
        //Resources
        resourceCounter.text = $"Resources: {ResourceManager.Instance.ResourceAmmount}";

        //Score
        scoreCounter.text = $"Score: {ScoreManager.Instance.Score:D5}";

        //Wave
        waveCounter.text = $"Wave: {waveManager.CurrentWave}";

        //Wave timer

        float t = Mathf.Max(0f, EnemyWaveManager.Instance.CurrentWaveTimer);
        int minutes = (int)(t / 60f);
        int seconds = (int)(t  % 60f);
        int milliseconds = (int)((t - Mathf.Floor(t)) * 1000f);

        timerCounter.text = $"Enemies Spawning for: {minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
