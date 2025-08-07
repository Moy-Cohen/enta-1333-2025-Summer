using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameHUDDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceCounter;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    [SerializeField] private TextMeshProUGUI waveCounter;


    [SerializeField] private EnemyWaveManager waveManager;
    
    void Update()
    {
        //Resources
        resourceCounter.text = $"Resources: {ResourceManager.Instance.ResourceAmmount}";

        //Score
        scoreCounter.text = $"Score: {ScoreManager.Instance.Score:D5}";

        //Wave
        waveCounter.text = $"Wave: {waveManager.CurrentWave}";

        
    }
}
