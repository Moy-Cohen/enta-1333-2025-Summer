using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score => Mathf.FloorToInt(scoreFloat);
    [Header("Scoring Settings")]

    [SerializeField] private float scorePerSecond = 10f;
    [SerializeField] private int scorePerEnemyKill = 100;

    private float scoreFloat;



    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        scoreFloat = 0f;
    }

    
    void Update()
    {
        scoreFloat += scorePerSecond * Time.deltaTime;
    }

    public void AddKillScore()
    {
        scoreFloat += scorePerEnemyKill;
    }

    public void AddCustomScore(int amount)
    {
        scoreFloat += amount;
    }

    public void ResetScore()
    {
        scoreFloat = 0f;
    }
}
