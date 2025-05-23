using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class Pathfinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private List<GridNode> path = new List<GridNode>();

    private void Start()
    {
        FindPath(new Vector2Int(0, 0), new Vector2Int(9, 9));
    }

    public void FindPath(Vector2Int startPoint, Vector2Int endPoint)
    {
        path.Clear(); //Clear the list for the path

        var startingNode = gridManager.GetNode(startPoint.x, startPoint.y);
        var endingNode = gridManager.GetNode(endPoint.x, endPoint.y);

        if (startingNode == null || endingNode == null || !startingNode.Value.walkable || !endingNode.Value.walkable)
        {
            Debug.LogWarning("Invalid starting or end point");
            return;
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPoint);

        Dictionary<Vector2Int, Vector2Int> lastPoint = new Dictionary<Vector2Int, Vector2Int>();
        lastPoint[startPoint] = startPoint;

        while (queue.Count > 0)
        {
            Vector2Int currentNode = queue.Dequeue();
            if (currentNode == endPoint)
                break;

            foreach (Vector2Int neighborNode in GetNeighbors(currentNode))
            {
                if (!lastPoint.ContainsKey(neighborNode))
                {
                    GridNode? gridNode = gridManager.GetNode(neighborNode.x, neighborNode.y);
                    if (gridNode != null && gridNode.Value.walkable)
                    {
                        queue.Enqueue(neighborNode);
                        lastPoint[neighborNode] = currentNode;
                    }
                }
            }
        }

        if (lastPoint.ContainsKey(endPoint))
        {
            Vector2Int currentNode = endPoint;
            while (currentNode != startPoint)
            {
                GridNode? gridNode = gridManager.GetNode(currentNode.x, currentNode.y);
                path.Add(gridNode.Value);
                currentNode = lastPoint[currentNode];
            }

            path.Reverse();
            Debug.Log("Path found length" + path.Count);

            foreach (var gridNode in path)
            {
                Debug.Log(gridNode.Name + gridNode.WorldPosition);
            }
        }
        else
        {
            Debug.LogWarning("Path not found");
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int coords)
    {
        List<Vector2Int> neighborNodes = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int next = coords + dir;
            if (gridManager.GetNode(next.x, next.y) != null)
            {
                neighborNodes.Add(next);
            }
        }
        return neighborNodes;
    }

    private void OnDrawGizmos()
    {
        /*if (path != null || gridManager == null || !gridManager.IsInitialized)
        {
            return;
        }

        Gizmos.color = Color.black;
        float gizmoSize = gridManager.GridSettings.NodeSize * 0.3f;

        for (int i = 0; i < path.Count; i++)
        {
            var coord = path[i];
            var currNode = gridManager.GetNode(coord.x, coord.y);
            Gizmos.DrawCube(currNode.Value.WorldPosition + Vector3.up * 0.1f, Vector3.one *  gizmoSize);
        }*/
        Handles.color = Color.yellow;
        Handles.DrawLine(Vector3.zero, Vector3.one * 5f); // Should show in Scene

        if (path == null || path.Count < 2)
            return;

        Handles.color = Color.black;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 from = path[i].WorldPosition + Vector3.up * 0.1f;
            Vector3 to = path[i + 1].WorldPosition + Vector3.up * 0.1f;

            Handles.DrawAAPolyLine(5f, from, to); // 5f is thickness
        }
    }

}
