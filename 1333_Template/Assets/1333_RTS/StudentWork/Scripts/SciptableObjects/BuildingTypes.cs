using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypes", menuName ="ScriptableObjects/BuildingTypes")]
public class BuildingTypes : ScriptableObject
{
    internal readonly object prefab;
    public List<BuildingData> Buildings = new List<BuildingData>();

}


