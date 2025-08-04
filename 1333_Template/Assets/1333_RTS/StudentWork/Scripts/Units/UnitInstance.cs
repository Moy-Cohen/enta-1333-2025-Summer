using System;
using UnityEngine;


public enum UnitTeam
{
    Player,
    Enemy
}
public class UnitInstance : MonoBehaviour
{
    [Header("UnitSettings")]
    public UnitTeam Team { get; private set; }
    public UnitType UnitType {  get; private set; }
    public int Durability {get; private set; }
    public int Damage { get; private set; }
    public float MoveSpeed = 2f;

    private int laneIndex;
    private bool hasTakenHitThisFrame = false;

    /// <summary>
    /// Initialize unit with its type data and team assignment.
    /// </summary>
    /// 
    public void Initialize(UnitType unitType, UnitTeam team)
    {
        UnitType = unitType;
        Team = team;
        Durability = unitType.Durability;
        Damage = unitType.Damage;

        
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = (team == UnitTeam.Player) ? unitType.PlayerMaterial : unitType.EnemyMaterial;
        }

        gameObject.name = $"{unitType.UnitName} ({team}";
    }
    private void Start()
    {
        laneIndex = Mathf.RoundToInt(transform.position.z);
        LaneManager.Instance.RegisterUnit(laneIndex, this);
    }

    private void Update()
    {
        hasTakenHitThisFrame = false;
        MoveForward();
    }

    private void MoveForward()
    {
        
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    public void TakeHit(int damage = 1)
    {
        if (hasTakenHitThisFrame) return;

        Durability -= damage;
        hasTakenHitThisFrame = true;

        if (Durability <= 0)
        {
            if(Team == UnitTeam.Enemy && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddKillScore();
            }
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.UnregisterUnit(laneIndex, this);
        }

        if(Team == UnitTeam.Enemy && EnemyWaveManager.Instance != null)
        {
            EnemyWaveManager.Instance.OnEnemyDestroyed(this);
        }
    }

    

}
