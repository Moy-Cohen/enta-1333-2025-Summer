using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmyComposition", menuName = "ScriptableObjects/ArmyComposition")]
public class ArmyComposition : ScriptableObject
{
    [System.Serializable]
    public class UnitEntry
    {
        public UnitTypePrefab unitTypePrefab;
        public int UnitCount = 1;

    }

    public List<UnitEntry> Units = new List<UnitEntry>();
}
