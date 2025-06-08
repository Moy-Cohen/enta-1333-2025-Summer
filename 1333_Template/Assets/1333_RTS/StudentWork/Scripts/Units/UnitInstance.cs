using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class UnitInstance : UnitBase
{
    [Header("Movement")]
    [SerializeField] private float _movespeed = 1f;

    private AStarPathfinding _aStar;
    private PathfindingManager _pathfindingManager;
    private List<GridNode> _currentPath = new List<GridNode>();
    private int _pathIndex = 1;
    private Vector3? _targetPosition = null;
    private bool _isMoving = false;

    public bool IsMoving => _isMoving;
    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(AStarPathfinding pathfinder, UnitType unitType)
    {
        _aStar = pathfinder;
        _unitType = unitType;
    }

    public void Update() 
    {
        if (_isMoving)
        {
            Debug.Log($"[Update] {name} is moving to {_currentPath[_pathIndex].WorldPosition}");
        }

        if(!_isMoving ||  _currentPath == null || _currentPath.Count == 0 || _pathIndex <= _currentPath.Count)
        {
            return;
        }

        Vector3 nextPoint = new Vector3(_currentPath[_pathIndex].WorldPosition.x, transform.position.y, _currentPath[_pathIndex].WorldPosition.z);

        Vector3 direction = (nextPoint - transform.position).normalized;
        float step = _movespeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, step);

        if (Vector3.Distance(transform.position, nextPoint) < 0.05f)
        {
            _pathIndex++;
            if (_pathIndex >= _currentPath.Count)
            {
                _isMoving = false;
            }
        }


    }


    public void SetTarget(Vector3 worldPosition)
    {
        Debug.Log($"[SetTarget] {name} trying to move to {worldPosition}");
        if (_aStar == null)
        {
            Debug.LogError($"[SetTarget] Pathfinder is NULL for {name}");
            return;
        }

        
    }

    public  void SetTarget()
    {

    }


    public override void MoveToTarget(GridNode targetNode)
    {

    }
    /*[Header("Prefab Stuff")]
    private Transform _animatorParent;
    [SerializeField] private ParticleSystem _hurtParticles;

    private GameObject _animatedUnit;
    private PathfindingManager _pathfinder;
    private Animator _characterAnimator;
    private List<GridNode> _currentPath = new();
    private int pathIndex = 0;
    private Vector3? _targetedWorldPosition = null;
    private bool _isMoving = false;

    public bool IsMoving => _isMoving;
    public void Initialize(PathfindingManager pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
    }*/
}
