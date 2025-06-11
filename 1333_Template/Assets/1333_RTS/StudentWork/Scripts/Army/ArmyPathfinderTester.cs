using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyPathfinderTester : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AStarPathfinding _aStarPathfinding;
    [SerializeField] private List<ArmyComposition> _armyCompositions = new();
    [SerializeField] private int _patrolRange = 8;
    [SerializeField] private float _detectionRange = 4f;

    private readonly List<ArmyManager> _armies = new();

    public ArmyManager PlayerArmy => _armies.Count > 0 ? _armies[0] : null;
    private enum UnitState { Patrol, Follow, Command }
    private readonly Dictionary<UnitInstance, UnitState> _unitStates = new();
    private readonly Dictionary<UnitInstance, Vector3[]> _patrolPoints = new();
    private readonly Dictionary<UnitInstance, int> _patrolTargetIndex = new();
    private readonly Dictionary<UnitInstance, UnitInstance> _followTargets = new();
    private readonly Dictionary<UnitInstance, Vector3> _lastEnemyPos = new();

    private static readonly Color[] ArmyColors = new Color[]
    {
            Color.cyan, Color.red, Color.yellow, Color.green, Color.magenta, Color.blue, Color.white, Color.black
    };

    private void Start()
    {
        _aStarPathfinding = new AStarPathfinding(_gridManager);
        _armies.Clear();

        for (int i = 0; i < _armyCompositions.Count; i++)
        {
            //ArmyManager army = new ArmyManager { ArmyID = i + 1, GridManager = gridManager };

            ArmyManager army = new ArmyManager { ArmyID = i, GridManager = _gridManager };
            SpawnArmyUnits(army, _armyCompositions[i]);
            _armies.Add(army);

            Debug.Log($"[Army] Created army with ID = {army.ArmyID}");

        }

    }

    private void SpawnArmyUnits(ArmyManager army, ArmyComposition composition)
    {
        foreach (var entry in composition.units)
        {
            for (int i = 0; i < entry.UnitCount; i++)
            {
                int attempts = 0;
                int maxAttempts = 1000;
                Vector3 _spawnPos = Vector3.zero;
                bool found = false;
                int unitWidth = entry.unitTypePrefab.UnitType.Width;
                int unitHeight = entry.unitTypePrefab.UnitType.Height;
                while (!found && attempts < maxAttempts)
                {
                    int x = Random.Range(0, _gridManager.GridSettings.GridSizeX - unitWidth + 1);
                    int y = Random.Range(0, _gridManager.GridSettings.GridSizeY - unitHeight + 1);
                    if (IsRegionWalkable(x, y, unitWidth, unitHeight))
                    {
                        _spawnPos = _gridManager.GetNode(x, y).WorldPosition;
                        found = true;
                    }
                    attempts++;
                }
                if (!found)
                {
                    Debug.LogWarning($"Failed to find valid spawn position for unit {entry.unitTypePrefab.UnitType.name}.");
                    continue;
                }
                float nodeHeight = _gridManager.GridSettings.NodeSize; // usually 1
                Vector3 liftedPosition = _spawnPos + Vector3.up * (nodeHeight / 2.5f + 0.1f);
                GameObject go = Instantiate(entry.unitTypePrefab.Prefab, liftedPosition, Quaternion.identity);
                UnitInstance unit = go.GetComponent<UnitInstance>();
                unit.Initialize(_aStarPathfinding, entry.unitTypePrefab.UnitType);
                army.Units.Add(unit);
                //_unitStates[unit] = UnitState.Command;
                _unitStates[unit] = army.IsPlayer ? UnitState.Command : UnitState.Patrol;
                _patrolPoints[unit] = new Vector3[2]
                    {
                        GetRandomPatrolPoint(_spawnPos, unit.Width, unit.Height),
                        GetRandomPatrolPoint(_spawnPos, unit.Width, unit.Height)
                    };
                _patrolTargetIndex[unit] = 0;

                //unit.SetTarget(_patrolPoints[unit][0]);

                Debug.Log($"[ArmySpawn] Unit '{unit.name}' set to patrol toward {_patrolPoints[unit][0]}");
            }
        }
    }

    private bool IsRegionWalkable(int x, int y, int width, int height)
    {
        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                if (!_gridManager.GetNode(x + dx, y + dy).walkable)
                    return false;
            }
        }
        return true;
    }

    private Vector3 GetRandomPatrolPoint(Vector3 origin, int unitWidth, int unitHeight)
    {
        GridNode node = _gridManager.GetNodeFromWorldPosition(origin);
        float nodeSize = _gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        int x = Mathf.Clamp(Random.Range(nodeX - _patrolRange, nodeX + _patrolRange), 0, _gridManager.GridSettings.GridSizeX - 1);
        int y = Mathf.Clamp(Random.Range(nodeY - _patrolRange, nodeY + _patrolRange), 0, _gridManager.GridSettings.GridSizeY - 1);
        for (int tries = 0; tries < 20; tries++)
        {
            int tryX = Mathf.Clamp(x + Random.Range(-_patrolRange, _patrolRange), 0, _gridManager.GridSettings.GridSizeX - unitWidth);
            int tryY = Mathf.Clamp(y + Random.Range(-_patrolRange, _patrolRange), 0, _gridManager.GridSettings.GridSizeY - unitHeight);
            if (IsRegionWalkable(tryX, tryY, unitWidth, unitHeight))
                return _gridManager.GetNode(tryX, tryY).WorldPosition;
        }
        return node.WorldPosition;
    }

    private void Update()
    {
        for (int i = 0; i < _armies.Count; i++)
        {
            ArmyManager ownArmy = _armies[i];
            List<UnitInstance> enemyUnits = new();
            for (int j = 0; j < _armies.Count; j++)
            {
                if (i == j) continue;
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

            // Skip units controlled by player (Command state)
            if (state == UnitState.Command)
                continue;

            switch (state)
            {
                case UnitState.Patrol:
                    // Check for nearby enemies
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
                    // Lost target or invalid
                    if (!_followTargets.ContainsKey(unit) || _followTargets[unit] == null)
                    {
                        _unitStates[unit] = UnitState.Patrol;
                        break;
                    }

                    UnitInstance target = _followTargets[unit];

                    // If target moved, update destination
                    if (Vector3.Distance(_lastEnemyPos[unit], target.transform.position) > 0.5f)
                    {
                        _lastEnemyPos[unit] = target.transform.position;
                        unit.SetTarget(target.transform.position);
                    }

                    // Stop following if target is far
                    if (Vector3.Distance(unit.transform.position, target.transform.position) > _detectionRange * 2)
                    {
                        _unitStates[unit] = UnitState.Patrol;
                    }

                    break;
            }
        }

        
    }

    private void PatrolBehavior(UnitInstance unit)
    {
        Vector3[] points = _patrolPoints[unit];
        int idx = _patrolTargetIndex[unit];
        if (Vector3.Distance(unit.transform.position, points[idx]) < 0.2f)
        {
            idx = 1 - idx;
            _patrolTargetIndex[unit] = idx;
            points[idx] = GetRandomPatrolPoint(unit.transform.position, unit.Width, unit.Height);
            unit.SetTarget(points[idx]);
        }
        else if (!unit.IsMoving)
        {
            unit.SetTarget(points[idx]);
        }
    }

    private UnitInstance FindNearestEnemy(UnitInstance unit, List<UnitInstance> enemyUnits)
    {
        float minDist = _detectionRange;
        UnitInstance nearest = null;
        foreach (UnitInstance enemy in enemyUnits)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(unit.transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }
        return nearest;
    }

    public void SetUnitState(UnitInstance unit, string stateName)
    {
        if (!_unitStates.ContainsKey(unit)) return;

        if (System.Enum.TryParse(stateName, out UnitState newState))
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
        for (int armyIdx = 0; armyIdx < _armies.Count; armyIdx++)
        {
            ArmyManager army = _armies[armyIdx];
            Color color = ArmyColors[armyIdx % ArmyColors.Length];
            foreach (UnitInstance unit in army.Units)
            {
                if (unit == null || unit.CurrentPath == null || unit.CurrentPath.Count < 2)
                    continue;
                Gizmos.color = color;
                for (int i = 0; i < unit.CurrentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(unit.CurrentPath[i].WorldPosition, unit.CurrentPath[i + 1].WorldPosition);
                }
            }
        }
    }
}