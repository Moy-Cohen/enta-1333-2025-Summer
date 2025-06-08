using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform LayoutGroupParent;
    [SerializeField] private SelectBuildingButton ButtonPrefab;
    [SerializeField] private BuildingTypes BuildingData;

    // Start is called before the first frame update
    void Start()
    {
        foreach(BuildingData t in BuildingData.Buildings)
        {
            SelectBuildingButton button = Instantiate(ButtonPrefab, LayoutGroupParent);
            button.SetUp(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
