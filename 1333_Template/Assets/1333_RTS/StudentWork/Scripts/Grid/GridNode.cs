using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single cell on the grid, storing world position, pathfinding, and terrain data.
/// </summary>

// Represents each node on our grid. Brutally efficient
[System.Serializable]
public class GridNode 
{
    public string Name; // Grid Index
    public Vector3 WorldPosition;
    public bool IsWalkable;
    public int Weight;
    public TerrainType TerrainType;
    

    public int X;
    public int Y;

    public Color GizmoColor => TerrainType != null
                                ? TerrainType.GizmoColor
                                :Color.white;

    
    // Pathfinding data
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    public GridNode Parent;
    public GridNode CameFromNode;
    public bool IsOccupied = false;

    
}
