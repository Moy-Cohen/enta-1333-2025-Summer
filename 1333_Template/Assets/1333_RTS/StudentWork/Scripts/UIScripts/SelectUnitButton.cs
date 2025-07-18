using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectUnitButton : MonoBehaviour
{
    private UnitType unitType;
    private BarrackInstance barrackPrefab;
    private int laneIndex;
    private bool isBarrackButton;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;


    
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
            UnitSelectionManager.Instance.SetSelectedBarrack(barrackPrefab);
            Debug.Log("Barrack selected. Click a lane to place it.");
        }
        else
        {
            UnitSelectionManager.Instance.SetSelectedUnit(unitType);
            Debug.Log($"Selected unit: {unitType.name}. Click a lane to spawn.");
        }
    }

    
}
