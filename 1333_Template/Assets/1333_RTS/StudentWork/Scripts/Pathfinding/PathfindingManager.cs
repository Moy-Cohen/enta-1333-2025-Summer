using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private AStarPathfinding aStar;
    private LineRenderer lineRenderer;
    private GridNode startNode;
    private GridNode endNode;


    private void Start()
    {
        aStar = new AStarPathfinding(gridManager);
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.black;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gridManager.InitializedGrid();
            lineRenderer.positionCount = 0;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (gridManager.IsInitialized)
            {
                PickRandomStartEnd();
                List<GridNode> path = aStar.Findpath(startNode, endNode);
                DrawPath(path);
                Debug.Log($"Pathfinding from {startNode.Name} to {endNode.Name}");
            }
        }
    }

    private void PickRandomStartEnd()
    {
        List<GridNode> walkables = new List<GridNode>();
        for(int x = 0; x < gridManager.GridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridManager.GridSettings.GridSizeY; y++)
            {
                GridNode node = gridManager.GetNode(x, y);
                if (node != null && node.walkable)
                {
                    walkables.Add(node);
                } 
            }
        }

        if (walkables.Count < 2)
        {
            Debug.LogWarning("Not enough walkable nodes to select start and end.");
            return;
        }
        startNode = walkables[Random.Range(0, walkables.Count)];
        endNode = walkables[Random.Range(0, walkables.Count)];

        while (endNode == startNode)
        {
            endNode = walkables[Random.Range(0, walkables.Count)];

        }

    }


    public void DrawPath(List<GridNode> path)
    {
        if (path == null || path.Count == 0)
        {
            lineRenderer.positionCount = 0;
            Debug.Log("No path found.");
            return;
        }
        lineRenderer.positionCount = path.Count;
        int totalCost = 0;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].WorldPosition + Vector3.up * 0.1f);
            totalCost += path[i].Weight;
        }
        Debug.Log($"Path length: {path.Count}, Total movement cost: {totalCost}");
    }

    
}
