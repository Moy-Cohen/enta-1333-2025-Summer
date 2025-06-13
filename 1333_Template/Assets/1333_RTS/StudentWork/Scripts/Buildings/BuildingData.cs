using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public string BuildingName;
    public Sprite BuildingIcon;
    public GameObject BuildingPrefab;


    public Vector2Int footprintSize = new Vector2Int(1, 1);

}