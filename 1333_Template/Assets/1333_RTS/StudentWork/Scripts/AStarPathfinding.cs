using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Unity.Hierarchy;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.XR;

public class AStarPathfinding : PathfindingAlgorithm
{
    private GridManager gridManager;
    private List<GridNode> debugPath;

    public List<GridNode> DebugPath => debugPath;

    public void Initialize(GridManager gridManager)
    {
        this.gridManager = gridManager;
        
    }

    public override List<GridNode> Findpath(GridNode start, GridNode end)
    {
        debugPath = new List<GridNode>();
        foreach (GridNode n in debugPath)
        {
            n.GCost = int.MaxValue;
            n.HCost = 0;
            n.CameFromNode = null;
        }


        List<GridNode> pendingNodes = new List<GridNode>();
        HashSet<GridNode> visitedNodes = new HashSet<GridNode>();

        start.GCost = 0;
        start.HCost = GetHeuristicCost(start, end);
        start.CameFromNode = null;

        pendingNodes.Add(start);

        while (pendingNodes.Count > 0)
        {
            // Get the node with the lowest FCost
            GridNode currentNode = GetLowestFCostNode(pendingNodes);

            // If reachd the goal, build the path
            if (currentNode == end)
            {
                debugPath = ReconstructPath(end);
                Debug.Log("Path finished");
                return debugPath;
                
            }

            //Move the current node to the visited nodes list 
            pendingNodes.Remove(currentNode);
            visitedNodes.Add(currentNode);

            //Check neighbors
            foreach (GridNode neighbor in gridManager.GetNeighbors(currentNode))
            {
                if(!neighbor.walkable || visitedNodes.Contains(neighbor))
                {
                    continue;
                }
                int tentativeGCost = currentNode.GCost + neighbor.Weight;

                if (tentativeGCost < neighbor.GCost || !pendingNodes.Contains(neighbor)) {
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetHeuristicCost(neighbor, end);
                    neighbor.CameFromNode = currentNode;

                    if (!pendingNodes.Contains(neighbor))
                    {
                        pendingNodes.Add(neighbor);
                    }
                }

            }
        }
        if (debugPath == null)
        {
            Debug.LogWarning("Path not Found");

            return null;
        }
        return debugPath;

        
    }



    /*public override List<GridNode> Findpath(Vector3 startWorld, Vector3 endWorld)
    {
        GridNode start = gridManager.GetNodeFromWorldPosition(startWorld);
        GridNode end = gridManager.GetNodeFromWorldPosition(endWorld);
        return null;
    }*/

    private int GetHeuristicCost(GridNode a, GridNode b)
    {
        int dx = Mathf.Abs(Mathf.RoundToInt(a.WorldPosition.x - b.WorldPosition.x));
        int dz = Mathf.Abs(Mathf.RoundToInt(a.WorldPosition.z - b.WorldPosition.z));
        return  (dx + dz);
    }

    private GridNode GetLowestFCostNode(List<GridNode> nodes)
    {

        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogWarning("List is empty");
            return null;
        }
        GridNode lowest = nodes[0];
        foreach (var node in nodes)
        {
            if (node.FCost  < lowest.FCost || (node.FCost == lowest.FCost && node.HCost < lowest.HCost))
            {
                lowest = node;
            }
        }

        return lowest;
    }

    

    private List<GridNode> ReconstructPath(GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.CameFromNode;
        }

        path.Reverse();
        return path;
    }

    


}
