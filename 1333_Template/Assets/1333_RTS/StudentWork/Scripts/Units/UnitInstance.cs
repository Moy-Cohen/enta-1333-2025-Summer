using UnityEngine;


public enum UnitTeam
{
    Player,
    Enemy
}
public class UnitInstance : MonoBehaviour
{
    [Header("UnitSettings")]
    public UnitTeam Team;
    public UnitType UnitType;

    public float MoveSpeed;
    public int Durability;
    private int laneIndex;

    private bool hasTakenHitThisFrame = false;
    

    private void Start()
    {
        if (UnitType != null)
        {
            Durability = UnitType.MaxHp;
            MoveSpeed = UnitType.MoveSpeed;
        }
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
        Vector3 direction = Team == UnitTeam.Player ? Vector3.right : Vector3.left;
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    public void TakeHit()
    {
        if (hasTakenHitThisFrame) return;

        Durability--;
        hasTakenHitThisFrame = true;
        if (Durability <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.UnregisterUnit(laneIndex, this);
        }
    }

}
