using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackInstance : MonoBehaviour
{
    [Header("BarrackProperties")]
    public int Durability = 10;
    public int[] controlledLanes;

    [Header("Spawning Test")]
    public UnitInstance UnitPrefab;
    public float SpawnInterval = 5f;

    private bool isDestroyed = false;
    private float spawnTimer;


    public void Update()
    {
        if(isDestroyed || UnitPrefab == null) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= SpawnInterval)
        {
            
            spawnTimer = 0f;
        }
    }

    public void SpawnUnit(UnitType unitType, int lane)
    {
        if (System.Array.IndexOf(controlledLanes, lane) < 0)
        {
            Debug.LogWarning("Barrack does not control this lane.");
            return;
        }

        Vector3 spawnPos = GridManager.Instance.GetWorldPosition(0, lane);
        UnitInstance unit = Instantiate(unitType.Prefab, spawnPos, Quaternion.identity);
        unit.Team = UnitTeam.Player;
        unit.Initialize(unitType);

    }

    public bool  ControllsLane(int lane)
    {
        foreach (int l in controlledLanes)
        {
            if (l == lane) return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        Durability -= amount;
        if (Durability<= 0)
        {
            AudioManager.Instance.PlaySFX("BarrackDestroyed");
            DestroyBarrack();
        }
    }

    private void DestroyBarrack()
    {
        isDestroyed = true;
        //ToDo disable spawing when destroy
        gameObject.SetActive(false);
    }
}
