using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private AStarPathfinding aStar;
    private List<GridNode> currentPath;

    public GridNode startNode;
    public GridNode endNode;

    [SerializeField] private bool rerollOnR = true;


    private void Awake()
    {
        aStar = new AStarPathfinding();
        aStar.Initialize(gridManager);
    }
    private void Start()
    {
        
        
        

        PickRandomStartEnd();
        FindPath();
    }

    private void Update()
    {
        if (rerollOnR && Input.GetKeyDown(KeyCode.R))
        {
            gridManager.InitializedGrid();
            for(int x = 0 ; x < gridManager.GridSettings.GridSizeX; x++)
            {
                for (int y = 0; y < gridManager.GridSettings.GridSizeY; y++)
                {
                    GridNode node = gridManager.GetNode(x, y);
                    node.AssignNeighbors(gridManager.GetAllNodes(), new Vector2Int(gridManager.GridSettings.GridSizeX, gridManager.GridSettings.GridSizeY));
                }
            }
            PickRandomStartEnd();
            FindPath();
        }
    }

    private void PickRandomStartEnd()
    {
        List<GridNode> walkable = gridManager.AllNodes.FindAll(n => n.walkable);

        if (walkable.Count < 2)
        {
            Debug.LogWarning("Need at least TWO walkable tiles");
            return;
        }
        startNode = walkable[Random.Range(0, walkable.Count)];
        do
        {
            endNode = walkable[Random.Range(0, walkable.Count)];
        } while (endNode == startNode);
    }


    public void FindPath()
    {
        currentPath = aStar.Findpath(startNode, endNode);
        if (currentPath == null)
        {
            Debug.LogWarning("No Path Found");
            /*gridManager.InitializedGrid();
            PickRandomStartEnd();
            FindPath();*/
        }

        currentPath = aStar.debugPath;
    }

    void OnDrawGizmos()
    {
        if (gridManager == null) return; 
        float size = gridManager.GridSettings.NodeSize * 0.3f;
        Gizmos.color = Color.white;
        Gizmos.DrawCube(startNode.WorldPosition + Vector3.up * 0.05f, Vector3.one * size);

        Gizmos.color = Color.black;
        Gizmos.DrawCube(endNode.WorldPosition + Vector3.up * 0.05f, Vector3.one * size);
        if (currentPath == null || currentPath.Count == 0) return;

        

        


        Gizmos.color = Color.cyan;
        for (int i = 1;  i < currentPath.Count-1; i++)
        {
            GridNode n = currentPath[i];
            Gizmos.DrawCube(n.WorldPosition + Vector3.up * 0.05f, Vector3.one * size);
            Gizmos.DrawLine(currentPath[i-1].WorldPosition, n.WorldPosition);
        }

        Gizmos.DrawLine(currentPath[^2].WorldPosition, currentPath[^1].WorldPosition);
    }
}
