using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    public UnitType SelectedUnit;
    public BarrackInstance SelectedBarrack;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetSelectedUnit(UnitType unit)
    {
        SelectedUnit = unit;
        SelectedBarrack = null;
    }

    public void SetSelectedBarrack(BarrackInstance barrackPrefab)
    {
        SelectedBarrack = barrackPrefab;
        SelectedUnit = null;
    }

    public void ClearSelection()
    {
        SelectedUnit = null;
        SelectedBarrack = null;
    }
}
