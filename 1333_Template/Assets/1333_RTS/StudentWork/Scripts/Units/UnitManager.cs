using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages unit creation and assignment across different armies.
/// Handles spawn logic and references to army managers.
/// </summary>

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    // Dictionary of ArmyManagers, keyed by player ID
    private Dictionary<int, ArmyManager> _armyManagers;

    // Example: Gets the player's army (assumes ID 0 is the player)
    public ArmyManager PlayerArmy => _armyManagers.TryGetValue(0, out var army) ? army : null;

    /// <summary>
    /// Spawns a test unit at a random walkable grid node and logs the result.
    /// </summary>
    public void SpawnUnit(Transform parent)
    {
        if (!_gridManager.IsInitialized)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }

        GridNode spawnNode = GetRandomWalkableNode();
        if (spawnNode == null)
        {
            Debug.LogWarning("No available walkable spawn node.");
            return;
        }

        Debug.Log($"[SpawnUnit] Dummy unit spawned at ({spawnNode.X}, {spawnNode.Y}) - World Pos: {spawnNode.WorldPosition}");
        // TODO: Instantiate unit prefab here using UnitTypePrefab and parent.
    }

    /// <summary>
    /// Picks a random walkable node from the grid.
    /// </summary>

    private GridNode GetRandomWalkableNode()
    {
        int tries = 100;
        while (tries-- > 0)
        {
            int x = Random.Range(0, _gridManager.GridSettings.GridSizeX);
            int y = Random.Range(0, _gridManager.GridSettings.GridSizeY);
            GridNode node = _gridManager.GetNode(x, y);

            if(node !=  null && node.IsWalkable && !node.IsOccupied)
            {
                return node;
            }
        }
        return null;
    }
}
