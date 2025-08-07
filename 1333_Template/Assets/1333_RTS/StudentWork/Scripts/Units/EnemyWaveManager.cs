using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    
    [Header("Wave Timig")]
    public float BaseWaveDuration = 10f;
    public float DurationGrowthFactor = 1.1f;
    public float WaveCooldown = 5f;

    [Header("Spawn Settings")]
    public float BaseSpawnInterval = 4f;
    public float SpawnIntervalDecrease = 0.1f;
    public float MinSpawnInterval = 0.4f;
    public int BaseLanesAtOnce = 3;
    public int BaseEnemiesPerLane = 1;
    public float EnemiesPerLaneGrowth = 0.1f;

    [Header("Enemy Progression")]
    public int EarlyUnlockSpacing = 3;
    public int LateUnlockSpacing = 5;
    public int SoftResetReduction = 3;


    private float currentSpawnTimer;
    public float CurrentWaveTimer;
    private float currentSpawnInterval;
    private float currentWaveDuration;

    public int CurrentWave = 0;
    private int maxEnemyIndexUnlocked = 0;

    private bool isWaveActive = false;
    private float cooldownTimer;

    private int currentMaxLanes;

    private bool isCheatCodeActive = false;

    private readonly List<UnitInstance> activeEnemies = new();
    public static EnemyWaveManager Instance;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        currentMaxLanes = BaseLanesAtOnce;
        StartNextWave();
    }

    void Update()
    {
        if (isWaveActive)
        {
            CurrentWaveTimer -= Time.deltaTime;
            currentSpawnTimer += Time.deltaTime;

            if (CurrentWaveTimer >0f && currentSpawnTimer >= currentSpawnInterval)
            {
                SpawnEnemies();
                currentSpawnTimer = 0;
            }

            if (CurrentWaveTimer <= 0 && activeEnemies.Count == 0)
            {
                EndWave();
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                StartNextWave();
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            CheatCode();
            isCheatCodeActive = true;
        }
    }

    private void StartNextWave()
    {
        CurrentWave++;
        isWaveActive = true;

        //WaveCooldown duration scaling (sub-expoential)
        currentWaveDuration = BaseWaveDuration * Mathf.Pow(DurationGrowthFactor, Mathf.Log(CurrentWave + 1));

        //Spawn interval decreases to min
        currentSpawnInterval = Mathf.Max(MinSpawnInterval, BaseSpawnInterval - (CurrentWave - 1) * SpawnIntervalDecrease);

        // Dynamic enemy unlock pacing (early vs late)
        UnitType[] enemyTypes = UnitDatabase.Instance.EnemyUnits;
        int spacing = (maxEnemyIndexUnlocked < enemyTypes.Length / 2) ? EarlyUnlockSpacing : LateUnlockSpacing;

        bool unlockedNew = false;
        if(CurrentWave % spacing == 0 && maxEnemyIndexUnlocked < enemyTypes.Length -1)
        {
            maxEnemyIndexUnlocked++;
            unlockedNew = true;
        }

        //Lane scaling with soft reset
        int height = GridManager.Instance.GridSettings.GridSizeX;
        int targetLanes = Mathf.Min(BaseLanesAtOnce + Mathf.RoundToInt((CurrentWave / 5f)), height);
        if (unlockedNew)
        {
            // Soft Reset: reduce mas spawnable lanes slighlty
            currentMaxLanes = Mathf.Max(BaseLanesAtOnce, targetLanes - SoftResetReduction);
        }
        else
        {
            currentMaxLanes = Mathf.Min(currentMaxLanes + 1, targetLanes);
        }

        Debug.Log($"Wave {CurrentWave} started! Duration: {currentWaveDuration:F1}s, Spawn Interval: {currentSpawnInterval:F2}s, Max Lanes: {currentMaxLanes}");
        CurrentWaveTimer = currentWaveDuration;
    }

    private void EndWave()
    {
        isWaveActive = false;
        cooldownTimer = WaveCooldown;
        Debug.Log($"Wave {CurrentWave} ended, Nex wave starts in {WaveCooldown} seconds");
    }

    private void SpawnEnemies()
    {
        UnitType[] enemyTypes = UnitDatabase.Instance.EnemyUnits;

        int height = GridManager.Instance.GridSettings.GridSizeY;
        int xPos = GridManager.Instance.GridSettings.GridSizeX - 1;

        int lanesToSpawn = Random.Range(1, currentMaxLanes + 1);
        int enemiesPerLane = Mathf.CeilToInt(BaseEnemiesPerLane + CurrentWave * EnemiesPerLaneGrowth);

        List<int> chosenLanes = new List<int>();
        while (chosenLanes.Count < lanesToSpawn)
        {
            int lane = Random.Range(0, height);
            if (!chosenLanes.Contains(lane))
                chosenLanes.Add(lane);
        }

        foreach (int lane in chosenLanes)
        {
            UnitType randomEnemyType = enemyTypes[Random.Range(0, maxEnemyIndexUnlocked + 1)];

            Vector3 spawnPos = GridManager.Instance.GetWorldPosition(xPos, lane);
            Quaternion rotation = Quaternion.Euler(0, -90, 0);
            UnitInstance enemy = Instantiate(randomEnemyType.Prefab, spawnPos, rotation);

            enemy.Initialize(randomEnemyType, UnitTeam.Enemy);

            activeEnemies.Add(enemy);
            
        }
    }

    public void OnEnemyDestroyed(UnitInstance enemy)
    {
        activeEnemies.Remove(enemy);
    }

    private void CheatCode()
    {
         if(!isCheatCodeActive)
        {
            CurrentWave = 50;
            ResourceManager.Instance.AddResource(1000);
        }
    }


    public void LoadWave(int index, float timer) { CurrentWave = index; CurrentWaveTimer = timer; }
}
