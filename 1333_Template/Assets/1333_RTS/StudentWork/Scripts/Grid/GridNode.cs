using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents each node on our grid. Brutally efficient
[System.Serializable]
public class GridNode 
{
    public string Name; // Grid Index
    public Vector3 WorldPosition;
    public bool walkable;
    public int Weight;
    public TerrainType terrainType;
    public GridNode CameFromNode;

    public int X;
    public int Y;
    public Color GizmoColor => terrainType != null
                                ? terrainType.GizmoColor
                                :Color.white;

    

    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;

    [System.NonSerialized]
    public List<GridNode> Neighbors;


    public void AssignNeighbors(GridNode[,]grid, Vector2Int gridSize)
    {
        Neighbors = new List<GridNode>();

        Vector2Int[] directions =
        {
            new Vector2Int(0,1),
            new Vector2Int(1,0),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0),
        };

        foreach (var dir  in directions)
        {
            Vector2Int neighborPos = new Vector2Int(X,Y) + dir;

            if (neighborPos.x >= 0 && neighborPos.y >= 0 && neighborPos.x < gridSize.x && neighborPos.y < gridSize.y)
            {
                GridNode neighbor = grid[neighborPos.x, neighborPos.y];
                if (neighbor.walkable)
                {
                    Neighbors.Add(neighbor);
                }
            }
        }
    }
}
