using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSpawnController : MonoBehaviour
{
    [SerializeField] private UnitCarousel carousel;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private BarrackInstance barrackPrefab;

    private readonly KeyCode[] num =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
    };

    private void Update()
    {
        for (int i = 0; i < num.Length; i++)
        {
            if (Input.GetKeyDown(num[i]))
            {
                TrySpawn(i);
            }
        }
    }


    private void TrySpawn(int laneIndex)
    {
        UnitType unit = carousel.ActiveUnit;
        if (unit == null) return;

        if (unit.IsBarrackCard)
        {
            if (!ResourceManager.Instance.HasEnough(unit.UnitCost))
            {
                Debug.Log("Not enough resources!");
                return;
            }

            BarrackInstance current = gridManager.GetBarrackInLane(laneIndex);

            if (current != null && !current.IsDestroyed())
            {
                Debug.Log("Barrack not destroyed in that lane-group!");
                return;
            }

            gridManager.TryRebuildBarrack(barrackPrefab, laneIndex);

            ResourceManager.Instance.SpendResource(unit.UnitCost);

            return;
        }


        if (!ResourceManager.Instance.HasEnough(unit.UnitCost))
        {
            Debug.Log("Not enough resources!");
            return;
        }

        BarrackInstance barrack = gridManager.GetBarrackInLane(laneIndex);
        if (barrack == null || barrack.IsDestroyed())
        {
            Debug.Log("No active barrack in that lane!");
            return;
        }

        gridManager.SpawnUnitFromBarrack(unit, laneIndex);

        

    }
}
