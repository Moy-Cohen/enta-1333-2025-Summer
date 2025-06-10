
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private TerrainType defaultTerrainType;
    [SerializeField] private TerrainType[] terrainTypes;

    public GridSettings GridSettings => gridSettings;

    public List<GridNode> Path = new List<GridNode>();
    public HashSet<GridNode> Visited = new HashSet<GridNode>();
    public List<GridNode> Front = new List<GridNode>();

    private GridNode[,] gridNodes;
    

    [Header("Debug for editor plymode only")]
    [SerializeField] private List<GridNode> allNodes = new();
    public List<GridNode> AllNodes => allNodes;

    public bool IsInitialized { get; private set; } = false;


    public void InitializedGrid()
    {
        allNodes.Clear();
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
               Vector3 worldPos = gridSettings.UseXZPlane
                ? new Vector3(x, 0, y) * gridSettings.NodeSize
                : new Vector3(x, y, 0) * gridSettings.NodeSize;

               TerrainType terrain = terrainTypes[Random.Range(0, terrainTypes.Length)];

                GridNode node = new GridNode
                {
                    Name = $"{terrain.TerrainName}_{x}_{y}",
                    WorldPosition = worldPos,
                    terrainType = terrain,
                    walkable = terrain.Walkable,
                    Weight = terrain.MovementCost,
                    X = x,
                    Y = y,

                };
                
                allNodes.Add(node);
                gridNodes[x, y] = node;
                //Debug.Log("GridReady");

               
            }
        }

        IsInitialized = true;
        
    }


    public GridNode GetNode(int x, int y)
    {
        if (x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY)
        {
            return gridNodes[x, y];
        }

        return null;
    }

    public void SetWalkable(int x, int y, bool isWalkable)
    {
        GridNode node = gridNodes[x, y];
        node.walkable = isWalkable;
        gridNodes[x, y] = node;
    }


    private void OnDrawGizmos()
    {
        if(gridNodes == null || gridSettings == null) return;

        Gizmos.color = Color.green;

        for (int x = 0; x< gridSettings.GridSizeX; x++)
        {
            for(int y = 0;y< gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x,y];
                Gizmos.color = node.walkable ? node.GizmoColor: Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
    }






    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        Vector3 pos = node.WorldPosition;

        int x = Mathf.RoundToInt(pos.x / GridSettings.NodeSize);
        int y = Mathf.RoundToInt(pos.y / GridSettings.NodeSize);

        int[,] directions = new int[,]
        {
            { 0, 1 },
            { 1, 0 },
            { 0, -1 },
            { -1, 0 }
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            GridNode neighbor = GetNode(x + directions[i, 0], y + directions[i, 1]);
            if (neighbor != null && neighbor.walkable)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }



    public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        int x = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.x / GridSettings.NodeSize) : Mathf.RoundToInt(position.x / gridSettings.NodeSize);
        int y = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.z / GridSettings.NodeSize) : Mathf.RoundToInt(position.z / gridSettings.NodeSize);

        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);

        return GetNode(x, y);
    }

    

}
