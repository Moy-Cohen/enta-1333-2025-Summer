using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines visual and gameplay properties of a terrain type. Used by GridNodes.
/// </summary>

[CreateAssetMenu(fileName = "TerrainTypes", menuName = "ScriptableObjects/TerrainTypes")]
public class TerrainType : ScriptableObject
{
    [SerializeField] private string terrainName = "Default";
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private bool isWalkable = true;
    [SerializeField] private int movementCost = 1;

    public string TerrainName => terrainName;
    public Color GizmoColor => gizmoColor;
    public bool IsWalkable => isWalkable;
    public int MovementCost => movementCost;
}
