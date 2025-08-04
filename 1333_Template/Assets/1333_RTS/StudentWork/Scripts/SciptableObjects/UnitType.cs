using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the core stats and size of a unit type. Used to configure unit instances.
/// </summary>

[CreateAssetMenu(fileName = "UnitType", menuName = "ScriptableObjects/UnitType")]
public class UnitType : ScriptableObject
{
    [Header("Visual & Prefab")]
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private UnitInstance prefab;
    [SerializeField] private Sprite unitIcon;

    [Header("Materials")]
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Material enemyMaterial;

    [Header("Combat Stats")]
    [SerializeField] private int durability = 1;
    [SerializeField] private int damage = 1;

    [Header("Cost")]
    [SerializeField] private int unitCost = 10;

    [SerializeField] private bool isBarrackCard = false;

    public string UnitName => unitName;
    public UnitInstance Prefab => prefab;
    public Sprite UnitIcon => unitIcon;
    public Material PlayerMaterial => playerMaterial;
    public Material EnemyMaterial => enemyMaterial;
    public int Durability => durability;
    public int Damage => damage;
    public int UnitCost => unitCost;
    public bool IsBarrackCard => isBarrackCard;
}
