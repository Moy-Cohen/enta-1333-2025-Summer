using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnerTest : MonoBehaviour
{
    public UnitInstance PlayerUnitPrefab;
    public UnitInstance EnemyUnitPrefab;

    public Transform PlayerSpawnPoint;
    public Transform EnemySpawnPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(PlayerUnitPrefab, PlayerSpawnPoint.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(EnemyUnitPrefab, EnemySpawnPoint.position, Quaternion.identity);
        }
    }
}
