using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Base class for all unit types. Provides common interface and access to unit data.
/// </summary>

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitType _unitType;

    // Width of the unit in grid cells.
    public virtual int Width => _unitType != null ? _unitType.Width : 1;

    // Height of the unit in grid cells.
    public virtual int Height => _unitType != null ? _unitType.Height : 1;

    // Abstract method to move this unit to a given grid node.
    public abstract void MoveToTarget(GridNode targetNode);


}
