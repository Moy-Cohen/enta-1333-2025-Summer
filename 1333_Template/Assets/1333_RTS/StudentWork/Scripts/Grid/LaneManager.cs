using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    [SerializeField] private GridSettings gridSettings;

    private float laneEndx;

    private Dictionary<int, List<UnitInstance>> laneUnits = new();

    public int ActiveBarracks = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        laneEndx = gridSettings.GridSizeX + 1.5f;
    }

    private void Start()
    {
        BarrackInstance[] barracks = FindObjectsOfType<BarrackInstance>();
        ActiveBarracks = barracks.Length;
    }

    public void RegisterBarrack()
    {
        ActiveBarracks++;
    }

    public void UnregisterBarrack()
    {
        ActiveBarracks--;
        if(ActiveBarracks <= 0)
        {
            GameStatesManager.Instance.ShowGameOver();
        }
    }

    public void RegisterUnit(int lane, UnitInstance unit)
    {
        if (!laneUnits.ContainsKey(lane))
        {
            laneUnits[lane] = new List<UnitInstance>();
        }
        if (!laneUnits[lane].Contains(unit))
        {
            laneUnits[lane].Add(unit);
        }
    }
    public void UnregisterUnit(int lane, UnitInstance unit)
    {
        if (laneUnits.ContainsKey(lane))
        {
            laneUnits[lane].Remove(unit);
        }
    }

    public void UpdateLaneCombat()
    {
        List<(UnitInstance, UnitInstance)> combatPairs = new();

        foreach (var lane in laneUnits)
        {
            List<UnitInstance> units = lane.Value;

            for (int i = 0; i < units.Count; i++)
            {
                for (int j = i + 1; j < units.Count; j++)
                {
                    var a = units[i];
                    var b = units[j];

                    if (a == null || b == null) continue;
                    if (a.Team == b.Team) continue;

                    float distance = Vector3.Distance(a.transform.position, b.transform.position);
                    if (distance < 0.5f)
                    {
                        combatPairs.Add((a, b));
                    }
                }
            }
        }

        foreach (var pair in combatPairs)
        {
            if (pair.Item1 != null) pair.Item1.TakeHit();
            if(pair.Item2  != null) pair.Item2.TakeHit();
            AudioManager.Instance.PlaySFX("UnitClash");
        }

        HandleBarrackDamage();
        CullPlayerUnitsAtLaneEnd();
    }

    private void HandleBarrackDamage()
    {
        foreach (var lane in laneUnits)
        {
            List<UnitInstance> units = new List<UnitInstance>(lane.Value);

            foreach (UnitInstance unit in units)
            {
                if (unit == null) continue;

                if(unit.Team == UnitTeam.Enemy && unit.transform.position.x <= 0)
                {
                    BarrackInstance[] barracks = FindObjectsOfType<BarrackInstance>();

                    foreach(var barrack in barracks)
                    {
                        if (barrack.ControllsLane(lane.Key))
                        {
                            barrack.TakeDamage(1);
                            Destroy(unit.gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void CullPlayerUnitsAtLaneEnd()
    {
        foreach (var lane in laneUnits)
        {
            foreach(UnitInstance unit in lane.Value.ToArray())
            {
                if (unit == null) continue;

                if (unit.Team == UnitTeam.Player && unit.transform.position.x >= laneEndx)
                {
                    Destroy(unit.gameObject);
                    UnregisterUnit(lane.Key, unit);
                }
            }
        }
    }

    public IEnumerable<UnitInstance> GetAllUnits()
    {
        foreach (var list in laneUnits.Values)
        {
            foreach (var u in list)
            {
                if (u !=  null) yield return u;
            }
        }
    }

}
