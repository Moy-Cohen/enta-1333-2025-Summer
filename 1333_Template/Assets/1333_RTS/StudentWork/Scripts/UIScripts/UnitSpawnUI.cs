using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UnitSpawnUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private BarrackInstance barrackPrefab;
    [SerializeField] private bool includeBarrackButton = true;

    public int LaneIndex;


    private void Start()
    {
        UnitType[] playerUnits = UnitDatabase.Instance.PlayerUnits;

        foreach(UnitType unit in  playerUnits)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            SelectUnitButton selector = btn.GetComponent<SelectUnitButton>();
            selector.Initialize(unit, LaneIndex);
        }

        if(includeBarrackButton && barrackPrefab != null)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            SelectUnitButton selector = btn.GetComponent<SelectUnitButton>();
            selector.InitializeForBarrack(barrackPrefab,LaneIndex);
        }
    }
}
