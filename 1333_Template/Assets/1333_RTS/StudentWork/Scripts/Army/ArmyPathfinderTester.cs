using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyPathfinderTester : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private AStarPathfinding _aStarPathfinding;
    [SerializeField] private List<ArmyComposition> _armyComposition = new();
    [SerializeField] private int patrolRange = 0;
    [SerializeField] private float detectionRange = 5f;

    private readonly List<ArmyManager> _armies = new();
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
        //_aStarPathfinding = new AStarPathfinding(_GridManager);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
