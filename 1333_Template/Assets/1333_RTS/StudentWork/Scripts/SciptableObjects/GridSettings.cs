using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration data for initializing and displaying the world grid.
/// Used by GridManager to construct and scale the tilemap.
/// </summary>

[CreateAssetMenu(fileName = "GridSettings", menuName = "ScriptableObjects/GridSettings")]
public class GridSettings : ScriptableObject
{
    [SerializeField] private int gridSizeX = 12;
    [SerializeField] private int gridSizeY = 12;
    [SerializeField] private float nodeSize = 1f;
    [SerializeField] private bool useXZPlane = true;

    public int GridSizeX => gridSizeX;
    public int GridSizeY => gridSizeY;
    public float NodeSize => nodeSize;
    public bool UseXZPlane => useXZPlane;
}
