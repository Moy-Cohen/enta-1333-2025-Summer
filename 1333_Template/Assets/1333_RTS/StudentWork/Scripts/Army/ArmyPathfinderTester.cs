using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyPathfinderTester : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AStarPathfinding _aStarPathfinding;
    [SerializeField] private List<ArmyComposition> _armyComposition = new();
    [SerializeField] private int patrolRange = 8;
    [SerializeField] private float detectionRange = 5f;

    private readonly List<ArmyManager> _armies = new();
    public ArmyManager PlayerArmy => _armies.Count > 0 ? _armies[0] : null;
    private enum UnitState
    {
        Patrol,
        Follow,
        Command
    }
    private readonly Dictionary<UnitInstance, UnitState> _unitStates = new();
    private readonly Dictionary<UnitInstance, Vector3[]> _patrolPoints = new();
    private readonly Dictionary<UnitInstance, int> _patrolTargetIndex = new();
    private readonly Dictionary<UnitInstance, UnitInstance> _followTargets = new();
    private readonly Dictionary<UnitInstance, Vector3> _lastEnemyPos = new();

    private static readonly Color[] ArmyColors = new Color[]
    {
        Color.cyan,
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.white,
        Color.black
    };


    // Start is called before the first frame update
    private void Start()
    {
        _aStarPathfinding = new AStarPathfinding(_gridManager);
        _armies.Clear();

        for (int i = 0; i < _armyComposition.Count; i++)
        {
            ArmyManager army = new ArmyManager { ArmyID = i, GridManager = _gridManager };
            SpawnArmyUnits(army, _armyComposition[i]);
            _armies.Add(army);

            Debug.Log($"[Army] Created army with ID = {army.ArmyID}");
        }
    }

    private void SpawnArmyUnits(ArmyManager army, ArmyComposition comp)
    {
        foreach (var entry in comp.units)
        {
            for (int i = 0; i < entry.UnitCount; i++)
            {
                int attempts = 0;
                int maxAttempts = 1000;
                Vector3 spawnPoint = Vector3.zero;
                bool found = false;
                int unitWidth = entry.unitTypePrefab.UnitType.Width;
                int unitHeight = entry.unitTypePrefab.UnitType.Height;

                while(!found && attempts < maxAttempts)
                {
                    int x = Random.Range(0, _gridManager.GridSettings.GridSizeX - unitWidth + 1);
                    int y = Random.Range(0, _gridManager.GridSettings.GridSizeY - unitHeight + 1);
                    if(IsRegionWalkable(x, y, unitWidth, unitHeight))
                    {
                        spawnPoint = _gridManager.GetNode(x, y).WorldPosition;
                        found = true;
                    }
                    attempts ++;
                }
                if (!found)
                {
                    Debug.LogWarning($"Failed to find valid spawn position for unit {entry.unitTypePrefab.UnitType.name}.");
                    continue;
                }
                float nodeHeight = _gridManager.GridSettings.NodeSize; // usually 1
                Vector3 liftedPosition = spawnPoint + Vector3.up * (nodeHeight / 2.5f + 0.1f);
                GameObject go = Instantiate(entry.unitTypePrefab.Prefab, liftedPosition, Quaternion.identity);
                UnitInstance unit = go.GetComponent<UnitInstance>();
                unit.Initialize(_aStarPathfinding, entry.unitTypePrefab.UnitType);
                army.Units.Add(unit);
                _unitStates[unit] = UnitState.Command;
                _patrolPoints[unit] = new Vector3[2]
                    {
                        GetRandomPatrolPoint(spawnPoint, unit.Width, unit.Height),
                        GetRandomPatrolPoint(spawnPoint, unit.Width, unit.Height)
                    };
                _patrolTargetIndex[unit] = 0;

                //unit.SetTarget(_patrolPoints[unit][0]);

                Debug.Log($"[ArmySpawn] Unit '{unit.name}' set to patrol toward {_patrolPoints[unit][0]}");

            }
        }
    }

    private bool IsRegionWalkable(int x,  int y, int width, int height)
    {
        for (int dx = 0; dx < width; dx++)
        {
            for(int dy = 0; dy < height; dy++)
            {
                if(!_gridManager.GetNode(x+dx, y + dy).walkable)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private Vector3 GetRandomPatrolPoint(Vector3 origin, int unitWidth,  int unitHeight)
    {
        GridNode node = _gridManager.GetNodeFromWorldPosition(origin);
        float nodeSize = _gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        int x = Mathf.Clamp(Random.Range(nodeX - patrolRange, nodeX + patrolRange), 0, _gridManager.GridSettings.GridSizeX - 1);
        int y = Mathf.Clamp(Random.Range(nodeY - patrolRange, nodeY + patrolRange), 0, _gridManager.GridSettings.GridSizeY - 1);
        for (int tries = 0; tries < 20; tries++)
        {
            int tryX = Mathf.Clamp(x + Random.Range(-patrolRange, patrolRange), 0, _gridManager.GridSettings.GridSizeX - unitWidth);
            int tryY = Mathf.Clamp(y + Random.Range(-patrolRange, patrolRange), 0, _gridManager.GridSettings.GridSizeY - unitHeight);
            if (IsRegionWalkable(tryX, tryY, unitWidth, unitHeight))
                return _gridManager.GetNode(tryX, tryY).WorldPosition;
        }
        return node.WorldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i =0; i < _armies.Count; i++)
        {
            ArmyManager ownArmy = _armies[i];
            List<UnitInstance> enemyUnits = new List<UnitInstance>();
            for (int j = 0; j < _armies.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                enemyUnits.AddRange(_armies[j].Units.Select(x => x as UnitInstance));
            }
            UpdateArmyUnits(ownArmy, enemyUnits);
        }
    }

    private void UpdateArmyUnits(ArmyManager ownArmy, List<UnitInstance> enemyUnits)
    {
        foreach (UnitInstance unit in ownArmy.Units)
        {
            if (unit == null) continue;
            UnitState state = _unitStates[unit];
            switch (state)
            {
                case UnitState.Patrol:
                    UnitInstance enemy = FindNearestEnemy(unit, enemyUnits);
                    if (enemy != null)
                    {
                        _unitStates[unit] = UnitState.Follow;
                        _followTargets[unit] = enemy;
                        _lastEnemyPos[unit] = enemy.transform.position;
                        unit.SetTarget(enemy.transform.position);
                    }
                    else
                    {
                        PatrolBehavior(unit);
                    }
                    break;
                    case UnitState.Follow:
                    if (!_followTargets.ContainsKey(unit) ||  _followTargets[unit] == null)
                    {
                        _unitStates[unit] = UnitState.Patrol;
                        break;
                    }
                    UnitInstance target = _followTargets[unit];
                    if(Vector3.Distance(_lastEnemyPos[unit], target.transform.position) > 0.5f)
                    {
                        _lastEnemyPos[unit] = target.transform.position;
                        unit.SetTarget(target.transform.position);
                    }
                    if (Vector3.Distance(unit.transform.position, target.transform.position) > detectionRange * 2)
                    {
                        _unitStates[unit] = UnitState.Patrol;
                        break;
                    }
                    break;
                   case UnitState.Command:
                    break;
            }
        }
    }


    private void  PatrolBehavior(UnitInstance unit)
    {
        Vector3[] points = _patrolPoints[unit];
        int index = _patrolTargetIndex[unit];
        if (Vector3.Distance(unit.transform.position, points[index]) < 0.2f)
        {
            index = 1 - index;
            _patrolTargetIndex[unit] = index;
            points[index] = GetRandomPatrolPoint(unit.transform.position, unit.Width, unit.Height);
            unit.SetTarget(points[index]);
        }
        else if (!unit.IsMoving)
        {
            unit.SetTarget(points[index]);
        }
    }

    private UnitInstance FindNearestEnemy(UnitInstance unit, List<UnitInstance> enemyUnits)
    {
        float minDistance = detectionRange;
        UnitInstance nearestEnemy = null;
        foreach (UnitInstance enemy in enemyUnits)
        {
            if(enemy == null) continue;
            float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    public void SetUnitState(UnitInstance unit, string stateName)
    {
        if (!_unitStates.ContainsKey(unit)) return;
        if(System.Enum.TryParse(stateName, out UnitState newState))
        {
            _unitStates[unit] = newState;
            Debug.Log($"[State] Unit {unit.name} state set to {newState}");
        }
        else
        {
            Debug.LogWarning($"[State] Invalid state '{stateName}'");
        }
    }

    private void OnDrawGizmos()
    {
        for(int armyId = 0; armyId < _armies.Count; armyId++)
        {
            ArmyManager army = _armies[armyId];
            Color color = ArmyColors[armyId %  ArmyColors.Length];
            foreach(UnitInstance unit in army.Units)
            {
                if (unit == null || unit.CurrentPath == null || unit.CurrentPath.Count < 2) continue;
                Gizmos.color = color;
                for (int i = 0; i < unit.CurrentPath.Count; i++)
                {
                    Gizmos.DrawLine(unit.CurrentPath[i].WorldPosition, unit.CurrentPath[i+1].WorldPosition);
                }
                    
                
            }
        }
    }
}
