using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitType", menuName = "ScriptableObjects/UnitType")]
public class UnitType : ScriptableObject
{
    /// <summary>
    /// The width of the unit in grid cells
    /// </summary>

    [SerializeField] private int width = 1;

    /// <summary>
    /// The height of the unit in grid cells
    /// </summary>

    [SerializeField] private int height = 1;

    [SerializeField] private int maxHp = 1;
    [SerializeField] private int moveSpeed = 1;
    [SerializeField] private int damage = 1;
    [SerializeField] private int defense = 1;
    
    [SerializeField] private AttackType attackType;
    [SerializeField] private int range = 1;

    public int Width => width;
    public int Height => height;
    public int MaxHp => maxHp;
    public int MoveSpeed => moveSpeed;
    public int Damage => damage;
    public int Defense => defense;
    public AttackType AttackType => attackType;
    public int Range => range;
}
