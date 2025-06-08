using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager
{
    public int ArmyID;
    public bool IsPlayer => ArmyID == 0;
    public List<UnitBase> Units = new List<UnitBase>();

    public GridManager GridManager;

    public void MoveAllUnits(Vector3 worldPosition)
    {
        foreach (var unit in Units)
        {
            unit.MoveToTarget(GridManager.GetNodeFromWorldPosition(worldPosition));
        }
    }

    public void MoveAllUnits(GridNode node)
    {
        foreach (var unit in Units)
        {
            unit.MoveToTarget(node);
        }
    }

}
