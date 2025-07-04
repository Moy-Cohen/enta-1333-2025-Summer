using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central controller that manages core game systems such as the grid and units.
/// Initializes necessary managers at game start.
/// </summary>


public class GameManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private UnitManager _unitManager;
    /*[SerializeField] private Pathfinder pathfinder;*/

    /// <summary>
    /// Called before the game starts.
    /// Initializes the grid for gameplay.
    /// </summary>
    private void Awake()
    {
        if (_gridManager != null)
        {
            _gridManager.InitializeGrid();
        }
        else
        {
            Debug.LogError("GridManager not assigned in GameManager.");
        }
    }


    private void Update()
    {
        LaneManager.Instance.UpdateLaneCombat();
    }

}
