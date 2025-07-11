
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

/// <summary>
/// Manages the 2D tile-based grid and all GridNode data.
/// Handles initialization, terrain assignment, and node lookups.
/// </summary>

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Configuration")]
    [SerializeField] private GridSettings _gridSettings;
    [SerializeField] private GameObject barrackPrefab;

    [Header("Terrain Settings")]
    [SerializeField] private TerrainType _defaultTerrainType;
    [SerializeField] private TerrainType[] _terrainTypes;

    public GridSettings GridSettings => _gridSettings;


    // Used for debug/visualization or pathfinding.
    public List<GridNode> Path = new();
    public HashSet<GridNode> Visited = new();
    public List<GridNode> Front = new();

    private GridNode[,] _gridNodes;
    

    [Header("Edittor Debugging")]
    [SerializeField] private List<GridNode> _allNodes = new();
    public List<GridNode> AllNodes => _allNodes;

    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnBarracks();
    }


    /// <summary>
    /// Initializes the grid with randomized terrain types and walkability settings.
    /// </summary>

    public void InitializeGrid()
    {
        _allNodes.Clear();
        _gridNodes = new GridNode[_gridSettings.GridSizeX, _gridSettings.GridSizeY];    

        for (int x = 0; x < _gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < _gridSettings.GridSizeY; y++)
            {
                GridNode node = CreateNode(x, y);
                _gridNodes[x,y] = node;
                _allNodes.Add(node);
               
            }
        }

        IsInitialized = true;
        
    }


    public Vector3 GetWorldPosition(int x, float y)
    {
        return new Vector3(x * _gridSettings.NodeSize, 0, y * _gridSettings.NodeSize);
    }


    private void SpawnBarracks()
    {
        int lanesPerBarrack = 3;
        int totalBarraks = _gridSettings.GridSizeX / lanesPerBarrack;

        for (int i = 0; i < totalBarraks; i++)
        {
            int laneStart = i * lanesPerBarrack;
            float centerLane = laneStart + 1;

            Vector3 spawnPos = GetWorldPosition(-1, centerLane);
            Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
            GameObject barrack = Instantiate(barrackPrefab, spawnPos, rotation);
            barrack.transform.localScale = Vector3.one * 0.5f;

            BarrackInstance instance = barrack.GetComponent<BarrackInstance>();
            if (instance != null)
            {
                instance.controlledLanes = new int[] {laneStart,  laneStart + 1, laneStart + 2};
            }
        }
    }

    public void TryRebuildBarrack(BarrackInstance prefab, int lane)
    {
        BarrackInstance existing = GetBarrackInLane(lane);
        if (existing != null) return;

        int startLane = lane - (lane % 3);
        float centerLane = startLane + 1;

        Vector3 spawnPos = GetWorldPosition(-1,centerLane);
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        GameObject barrack = Instantiate(barrackPrefab, spawnPos, rotation);
        barrack.transform.localScale = Vector3.one * 0.5f;

        BarrackInstance instance = barrack.GetComponent<BarrackInstance>();
        if (instance != null)
        {
            instance.controlledLanes = new int[] {startLane, startLane + 1, startLane + 2};
        }
    }

    public BarrackInstance GetBarrackInLane(int lane)
    {
        BarrackInstance[] allBarracks = FindObjectsOfType<BarrackInstance>();
        
        foreach(var barrack in allBarracks)
        {
            foreach(int l in barrack.controlledLanes)
            {
                if (l == lane) return barrack;
            }
        }
        return null;
    }


    /// <summary>
    /// Creates a new GridNode with assigned terrain and properties.
    /// </summary>
    private GridNode CreateNode(int x, int y)
    {
        Vector3 worldPos = _gridSettings.UseXZPlane
                ? new Vector3(x, 0, y) * _gridSettings.NodeSize
                : new Vector3(x, y, 0) * _gridSettings.NodeSize;

        TerrainType terrain = _terrainTypes[Random.Range(0, _terrainTypes.Length)];

        return new GridNode
        {
            Name = $"{terrain.TerrainName}_{x}_{y}",
            WorldPosition = worldPos,
            TerrainType = terrain,
            IsWalkable = terrain.IsWalkable,
            Weight = terrain.MovementCost,
            X = x,
            Y = y,
        };
    }

    /// <summary>
    /// Returns the GridNode at the given grid coordinates.
    /// </summary>
    public GridNode GetNode(int x, int y)
    {
        if (x >= 0 && x < _gridSettings.GridSizeX && y >= 0 && y < _gridSettings.GridSizeY)
        {
            return _gridNodes[x, y];
        }

        return null;
    }

    /// <summary>
    /// Sets whether the node at the specified grid coordinates is walkable.
    /// </summary>
    public void SetWalkable(int x, int y, bool isWalkable)
    {
        GridNode node = _gridNodes[x, y];
        node.IsWalkable = isWalkable;
        _gridNodes[x, y] = node;
    }

    private void OnDrawGizmos()
    {
        if(_gridNodes == null || _gridSettings == null) return;

        for (int x = 0; x< _gridSettings.GridSizeX; x++)
        {
            for(int y = 0;y< _gridSettings.GridSizeY; y++)
            {
                GridNode node = _gridNodes[x,y];
                Gizmos.color = node.IsWalkable ? node.GizmoColor: Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * _gridSettings.NodeSize * 0.9f);
            }
        }
    }


    public List<GridNode> GetAllNodes()
    {
        List<GridNode> all = new();
        for(int x = 0; x <GridSettings.GridSizeX; x++)
        {
            for(int y = 0; y < GridSettings.GridSizeY; y++)
            {
                all.Add(_gridNodes[x, y]);
            }

        }
        return all;
    }

    /// <summary>
    /// Returns a list of walkable neighboring nodes for a given node.
    /// </summary>
    public List<GridNode> GetNeighbors(GridNode node)
    {
        Debug.Log($"GetNeighbors CURRENT {node.Name} WorldPosition {node.WorldPosition}");
        List<GridNode> neighbors = new List<GridNode>();
        Vector3 pos = node.WorldPosition;

        int x = Mathf.RoundToInt(pos.x / _gridSettings.NodeSize);
        int y = Mathf.RoundToInt(pos.y / _gridSettings.NodeSize); // XZ plane assumed

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
            if (neighbor != null && neighbor.IsWalkable && node != neighbor)
            {
                neighbors.Add(neighbor);
                Debug.Log($"GetNeighbors NEIGHBOR {neighbor.Name} in {neighbor.WorldPosition}");
            }
        }
        return neighbors;
    }


    /// <summary>
    /// Finds the node on the grid closest to a given world position.
    /// </summary>
    public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        int x = _gridSettings.UseXZPlane 
            ? Mathf.RoundToInt(position.x / _gridSettings.NodeSize) 
            : Mathf.RoundToInt(position.x / _gridSettings.NodeSize);


        int y = _gridSettings.UseXZPlane 
            ? Mathf.RoundToInt(position.z / _gridSettings.NodeSize) 
            : Mathf.RoundToInt(position.z / _gridSettings.NodeSize);

        x = Mathf.Clamp(x, 0, _gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, _gridSettings.GridSizeY - 1);

        return GetNode(x, y);
    }

    /// <summary>
    /// Clamps a world position to the nearest grid boundary.
    /// </summary>
    public Vector3 ClampWorldToGrid(Vector3 worldPos)
    {
        float nodeSize = _gridSettings.NodeSize;
        int maxX = _gridSettings.GridSizeX - 1;
        int maxY = _gridSettings.GridSizeY - 1;

        float clampedX = Mathf.Clamp(worldPos.x, 0, maxX * nodeSize);
        float clampedZ = Mathf.Clamp(worldPos.z, 0, maxY * nodeSize);

        return new Vector3(clampedX, worldPos.y, clampedZ);
    }

    /// <summary>
    /// Returns a random walkable node from the grid.
    /// </summary>
    public GridNode GetRandomWalkableNode()
    {
        List<GridNode> walkableNodes = new List<GridNode>();

        for (int x = 0; x < _gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < _gridSettings.GridSizeY; y++)
            {
                GridNode node = GetNode(x, y);
                if (node != null && node.IsWalkable)
                {
                    walkableNodes.Add(node);
                }
            }
        }

        if (walkableNodes.Count == 0)
        {
            Debug.LogWarning("[GridManager] No walkable nodes found.");
            return null;
        }

        return walkableNodes[Random.Range(0, walkableNodes.Count)];
    }

}
