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
            SpawnUnit();
            spawnTimer = 0f;
        }
    }

    private void SpawnUnit()
    {
        int randomLane = controlledLanes[Random.Range(0, controlledLanes.Length)];
        Vector3 spawnPos = GridManager.Instance.GetWolrdPosition(0, randomLane);
        //spawnPos.x = transform.position.x + 1f;
        
        UnitInstance unit = Instantiate(UnitPrefab, spawnPos, Quaternion.identity);
        unit.Team = UnitTeam.Player;
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
