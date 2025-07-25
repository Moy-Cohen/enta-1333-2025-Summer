using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrackInstance : MonoBehaviour
{
    [Header("BarrackProperties")]
    public int Durability = 10;
    public int BarrackCost = 25;
    public int[] controlledLanes;

    [Header("Spawning Test")]
    public UnitInstance UnitPrefab;
    public float SpawnInterval = 5f;

    [Header("Models")]
    [SerializeField] private GameObject intactModel;
    [SerializeField] private GameObject destroyedModel;

    private bool isDestroyed = false;
    private float spawnTimer;


    private void Start()
    {
        if (intactModel != null) intactModel.SetActive(true);
        if (destroyedModel !=  null) destroyedModel.SetActive(false);
    }

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
        if (isDestroyed || UnitPrefab == null) return;
        if (System.Array.IndexOf(controlledLanes, lane) < 0)
        {
            Debug.LogWarning("Barrack does not control this lane.");
            return;
        }

        Vector3 spawnPos = GridManager.Instance.GetWorldPosition(0, lane);
        UnitInstance unit = Instantiate(unitType.Prefab, spawnPos, Quaternion.identity);
        unit.Initialize(unitType, UnitTeam.Player);

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
        if(isDestroyed) return;
        isDestroyed = true;

        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.UnregisterBarrack();
        }


        if (intactModel != null) intactModel.SetActive(false);
        if (destroyedModel != null) destroyedModel.SetActive(true);
    }

    public void RebuildBarrack()
    {
        if (!isDestroyed) return;

        if (!ResourceManager.Instance.SpendResource(BarrackCost))
        {
            Debug.Log("Not enough resources to rebuild barrack.");
            return;
        }

        isDestroyed = false;
        Durability = 10;

        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.RegisterBarrack();
        }

        if (intactModel != null) intactModel.SetActive(true);
        if (destroyedModel != null) destroyedModel.SetActive(false);

        AudioManager.Instance.PlaySFX("BarrackPlaced");
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }
}
