using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public static UnitDatabase Instance;

    [Header("Player Units")]
    public UnitType[] PlayerUnits;

    [Header("Enemy Units (ordered by difficulty)")]
    public UnitType[] EnemyUnits;


    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
}
