using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class UnitInstance : UnitBase
{
    [Header("Prefab Stuff")]
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
    }
}
