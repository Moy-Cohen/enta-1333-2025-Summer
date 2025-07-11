using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectUnitButton : MonoBehaviour
{
    private UnitType unitType;
    private BarrackInstance barrackPrefab;
    private int laneIndex;
    private bool isBarrackButton;

    [SerializeField] private Button button;
    [SerializeField] private Text label;

    public void Initialize(UnitType unit, int lane)
    {
        unitType = unit;
        laneIndex = lane;
        isBarrackButton = false;

        label.text = unit.name;
        button.onClick.AddListener(OnClick);
    }

    public void InitializeForBarrack(BarrackInstance prefab,int lane)
    {
        barrackPrefab = prefab;
        laneIndex = lane;
        isBarrackButton = true;

        label.text = $"Rebuild Barrack (Lane {lane})";
        button.onClick.AddListener(OnClick);
    }


    private void OnClick()
    {
        if (isBarrackButton)
        {
            if(GridManager.Instance.GetBarrackInLane(laneIndex) != null)
            {
                Debug.Log("Barrack already exists in this lane.");
                return;
            }

            if (ResourceManager.Instance.SpendResource(100))
            {
                GridManager.Instance.TryRebuildBarrack(barrackPrefab, laneIndex);
                AudioManager.Instance.PlaySFX("BarrackPlaced");
            }
        }
        else
        {
            BarrackInstance barrack = GridManager.Instance.GetBarrackInLane(laneIndex);
            if (barrack == null)
            {
                Debug.LogWarning("No barrack in this lane to spawn unit.");
                return;
            }

            if (ResourceManager.Instance.SpendResource(unitType.UnitCost))
            {
                barrack.SpawnUnit(unitType, laneIndex);
                AudioManager.Instance.PlaySFX("PlayerUnitSpawn");
            }
        }
    }

    
}
