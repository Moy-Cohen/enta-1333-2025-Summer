using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a collection of unit types and quantities to spawn as part of an army.
/// Used by ArmyPathfinderTester for initializing test armies.
/// </summary>

[CreateAssetMenu(fileName = "ArmyComposition", menuName = "ScriptableObjects/ArmyComposition")]
public class ArmyComposition : ScriptableObject
{
    [System.Serializable]
    public class UnitEntry
    {
        [SerializeField, Tooltip("The unit type and prefab reference.")]
        private UnitTypePrefab unitTypePrefab;

        [SerializeField, Tooltip("How many units of this type to spawn.")]
        private int unitCount = 1;

        public UnitTypePrefab UnitTypePrefab => unitTypePrefab;
        public int UnitCount => unitCount;
    }

    [Tooltip("List of unit types and their counts for this army.")]
    public List<UnitEntry> Entries = new List<UnitEntry>();
}
