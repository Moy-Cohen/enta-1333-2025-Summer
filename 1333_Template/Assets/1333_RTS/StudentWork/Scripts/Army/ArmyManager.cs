using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a group of units (player or AI). Can issue commands to the entire army.
/// </summary>
public class ArmyManager
{
    // Unique ID to identify this army. ID 0 = player.
    public int ArmyID;

    // Returns true if this army belongs to the player.
    public bool IsPlayer => ArmyID == 0;

    // List of units in this army.
    public List<UnitBase> Units = new();

    // GridManager reference for pathfinding and node lookup.
    public GridManager GridManager;

    // Moves all units to a world-space position by converting it to a grid node.
    public void MoveAllUnits(Vector3 worldPosition)
    {
        foreach (var unit in Units)
        {
            unit.MoveToTarget(GridManager.GetNodeFromWorldPosition(worldPosition));
        }
    }

    // Moves all units to the specified grid node.
    public void MoveAllUnits(GridNode node)
    {
        foreach (var unit in Units)
        {
            unit.MoveToTarget(node);
        }
    }

}
