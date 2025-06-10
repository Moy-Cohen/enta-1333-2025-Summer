using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        transform.position = _aStar._GridManager.GetNodeFromWorldPosition(transform.position).WorldPosition;
        _currentPath = _aStar.Findpath(transform.position, worldPosition);
        if (_currentPath == null )
        {
            Debug.LogError($"[SetTarget] {name} path is NULL.");
            return;
        }
        if (_currentPath.Count <= 1)
        {
            Debug.LogWarning($"[SetTarget] {name} path too short. Count = {_currentPath.Count}");
            return;
        }

        _pathIndex = 0;
        _targetPosition = worldPosition;
        _isMoving = true;
        Debug.Log($"[SetTarget] {name} path assigned with {_currentPath.Count} nodes");

        for (int i = 0; i < _currentPath.Count - 1;  i++)
        {
            Debug.DrawLine(
                _currentPath[i].WorldPosition + Vector3.up * 1f,
                _currentPath[i + 1].WorldPosition + Vector3.up * 1f,
                Color.cyan, 5f
            );
        }
    } 
    

    public  void SetTarget(GridNode node)
    {
        SetTarget(node.WorldPosition);
    }


    public override void MoveToTarget(GridNode targetNode)
    {
        SetTarget(targetNode);
    }
    
}
