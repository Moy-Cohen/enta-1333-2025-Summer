using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public UnitInstance EnemyUnitPrefab;
    public float SpawnInterval = 7f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >=  SpawnInterval)
        {
            SpawnWave();
            spawnTimer = 0f;
        }
    }

    private void SpawnWave()
    {
        if (EnemyUnitPrefab == null) return;
        
        int height = GridManager.Instance.GridSettings.GridSizeX;
        int xPos = GridManager.Instance.GridSettings.GridSizeY - 1;

        for (int lane = 0; lane < height; lane++)
        {
            Vector3 SpawnPos = GridManager.Instance.GetWorldPosition(xPos, lane);
            Quaternion rotation = Quaternion.Euler(0, -90, 0);
            UnitInstance enemy = Instantiate(EnemyUnitPrefab, SpawnPos, rotation);
            enemy.Team = UnitTeam.Enemy;
        }
    }
}
