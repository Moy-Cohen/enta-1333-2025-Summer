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

    private GridNode[,] gridNodes;

    [Header("Debug for editor plymode only")]
    [SerializeField] private List<GridNode> allNodes = new();
    public List<GridNode> AllNodes => allNodes;

    public bool IsInitialized { get; private set; } = false;

    public void InitializedGrid()
    {
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
            }
        }
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


    public GridNode GetNode(int x, int y) 
    {
        if(x < 0 || x >= gridSettings.GridSizeX || y < 0  || y >= gridSettings.GridSizeY)
        {
            return null ;
        }
        
        return gridNodes[x,y];
            
        
    }

    public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        //Determine which axes to use baedon grid orientation 
        int x = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.x / gridSettings.NodeSize) : Mathf.RoundToInt(position.x / gridSettings.NodeSize);
        int y = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.z / gridSettings.NodeSize) : Mathf.RoundToInt(position.y / gridSettings.NodeSize);
        //Clamp coordinates to grid bounds.
        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);
        //Return the node at the clamped coordinates.
        return GetNode(x,y);
    }

    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();

        // 4 Directional Movements (Up, Right. Down, Left)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0,1),  //Up
            new Vector2Int(1,0),  //Right
            new Vector2Int(0,-1), //Down
            new Vector2Int(-1,0), //Left
        };

        foreach (Vector2Int dir in directions)
        {
            int nx = node.X + dir.x;
            int ny = node.Y + dir.y;

            GridNode neighbor = GetNode(nx, ny);

            if (neighbor != null && neighbor.walkable)
            {
                neighbors.Add(neighbor);
            } 
        }
        return neighbors;
    }

}
