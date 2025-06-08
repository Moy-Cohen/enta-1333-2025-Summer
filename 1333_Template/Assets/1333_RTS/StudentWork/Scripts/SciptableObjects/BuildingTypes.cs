using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypes", menuName ="ScriptableObjects/BuildingTypes")]
public class BuildingTypes : ScriptableObject
{
    public List<BuildingData> Buildings = new List<BuildingData>();
}

[System.Serializable]
public class BuildingData
{
    public string BuildingName;
    public Sprite BuildingIcon;
}
