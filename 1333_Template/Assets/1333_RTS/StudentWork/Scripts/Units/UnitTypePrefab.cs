using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Associates a UnitType definition with its corresponding visual prefab.
/// Used for spawning and instantiating unit visuals.
/// </summary>

[System.Serializable]
public class UnitTypePrefab
{
    [Tooltip("The UnitType that this prefab represents.")]
    public UnitType UnitType;

    [Tooltip("The prefab GameObject to instantiate for this unit.")]
    public GameObject Prefab;
}

