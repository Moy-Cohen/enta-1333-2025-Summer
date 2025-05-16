using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private TerrainType defaultTerrainType;

    public GridSettings GridSettings => gridSettings;

    private GridNode[,] gridNodes;

    [Header("Debug for editor plymode only")]
    [SerializeField] private List<GridNode> AllNodes = new();

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

                GridNode node = new GridNode
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = worldPos,
                    walkable = true,
                    Weight = 1,

                };
                //AllNodes.Add(node);
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
                Gizmos.color = node.walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
    }

    
}
