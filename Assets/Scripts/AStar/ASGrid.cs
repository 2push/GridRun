using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASGrid : MonoBehaviour {
    public LayerMask unwalkableMask;
    ASNode[,] grid;
    Vector2 gridWorldSize;
    float nodeD;
    Vector3 gridCenterPosition;
    int gridSizeX;
    int gridSizeY;

    public void ProceedLevel(AStarGenerationData generationData)
    {
        StartCoroutine(PauseForLevelObjectsUpdate(generationData));
    }
    
    private IEnumerator PauseForLevelObjectsUpdate(AStarGenerationData generationData)
    {
        yield return new WaitForEndOfFrame();
        Init(generationData);
        GenerateGrid();
    }

    private void Init(AStarGenerationData genData)
    {
        grid = null;
        gridWorldSize = genData.gridWorldSize;
        nodeD = genData.nodeDiameter;
        gridCenterPosition = genData.gridCenterPosition;
        gridSizeX = genData.nodesInGridX;
        gridSizeY = genData.nodesInGridY;
        grid = new ASNode[gridSizeX, gridSizeY];
    }

    public float GetNodeDiameter //for ASPathfinding to draw path gizmos
    {
        get { return nodeD; }
    }

    private void OnDrawGizmos()
    {      
        Gizmos.DrawWireCube(gridCenterPosition, new Vector3(gridWorldSize.x, nodeD, gridWorldSize.y)); 
        if (grid != null)
        {
                foreach (ASNode node in grid)
                {
                    Gizmos.color = node.walkable ? Color.grey : Color.red;
                    Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeD - Values.IndentBetweenNodes));
                }                   
        }
    }

    public  ASNode GetNodeFromWorldPoint(Vector3 worldPoint)
    {
        float posX = ((worldPoint.x - gridCenterPosition.x) + gridWorldSize.x * 0.5f) / nodeD;
        float posY = ((worldPoint.z - gridCenterPosition.z) + gridWorldSize.y * 0.5f) / nodeD;
        posX = Mathf.Clamp(posX, 0, gridWorldSize.x - 1);
        posY = Mathf.Clamp(posY, 0, gridWorldSize.y - 1);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);

        return grid[x, y];
    }

    public List<ASNode> GetNodeNeighbors(ASNode node)
    {
        List<ASNode> neighbours = new List<ASNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //exclude center node
                    continue;
                if (x != 0 && y != 0) // exclude diagonal nodes. Comment the statement to take them in
                    continue;
                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public void GenerateGrid()
    {
        Vector3 gridLeftBottomInWorld = gridCenterPosition - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = gridLeftBottomInWorld + Vector3.right * (x * nodeD + nodeD/2) +
                    Vector3.forward * (y * nodeD + nodeD/2);
                bool obstacleCollision = !(Physics.CheckSphere(worldPoint, nodeD/2, unwalkableMask));
                grid[x, y] = new ASNode(x,y, worldPoint, obstacleCollision);
            }
        }
    }  
}
