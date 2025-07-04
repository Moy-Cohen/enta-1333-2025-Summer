using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the core stats and size of a unit type. Used to configure unit instances.
/// </summary>

[CreateAssetMenu(fileName = "UnitType", menuName = "ScriptableObjects/UnitType")]
public class UnitType : ScriptableObject
{

    [SerializeField, Tooltip("Width of the unit in grid cells.")]
    private int width = 1;

    [SerializeField, Tooltip("Height of the unit in grid cells.")]
    private int height = 1;

    [SerializeField, Tooltip("Maximum health points.")]
    private int maxHp = 1;

    [SerializeField, Tooltip("How many tiles the unit moves per turn or cycle.")]
    private int moveSpeed = 1;

    [SerializeField, Tooltip("Base damage dealt per attack.")]
    private int damage = 1;

    [SerializeField, Tooltip("Flat defense value that reduces incoming damage.")]
    private int defense = 1;

    [SerializeField, Tooltip("Type of attack (melee, ranged, etc.).")]
    private AttackType attackType;

    [SerializeField, Tooltip("How many tiles away the unit can attack.")]
    private int range = 1;

    [SerializeField, Tooltip("Cost in resources to train this unit.")]
    private int unitCost = 10;

    [SerializeField, Tooltip("Time (in seconds or turns) required to train the unit.")]
    private float trainingTime = 5f;

    [SerializeField, Tooltip("Icon used to represent this unit in UI.")]
    private Sprite unitIcon;

    public int Width => width;
    public int Height => height;
    public int MaxHp => maxHp;
    public int MoveSpeed => moveSpeed;
    public int Damage => damage;
    public int Defense => defense;
    public AttackType AttackType => attackType;
    public int Range => range;
    public int UnitCost => unitCost;
    public float TrainingTime => trainingTime;
    public Sprite UnitIcon => unitIcon;
}
