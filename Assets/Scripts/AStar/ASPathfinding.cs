using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ASPathfinding : MonoBehaviour {

    private ASGrid grid;
    private List<ASNode> pathNodes;
    private List<Vector3> path;

    private void Start()
    {
        grid = GetComponent<ASGrid>();
    }

    public void FindPathFromAtoB(PathRequestData pathRequest)
    {
        ASNode startNode = grid.GetNodeFromWorldPoint(pathRequest.startPos);
        ASNode finishNode = grid.GetNodeFromWorldPoint(pathRequest.finishPos);
        if (finishNode.walkable == false)
            return;
        List<ASNode> openNodes = new List<ASNode>();
        HashSet<ASNode> closedNodes = new HashSet<ASNode>();
        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            ASNode currentNode = openNodes[0];
            for(int i = 1; i<openNodes.Count; i++)
            {
                if (openNodes[i].FCost < currentNode.FCost || openNodes[i].FCost == currentNode.FCost && openNodes[i].hCost < currentNode.hCost)
                {
                    if (openNodes[i].hCost < currentNode.hCost)
                        currentNode = openNodes[i];
                }      
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == finishNode)
            {
                RetracePath(startNode, finishNode);
                pathRequest.pathGetter(path);
                return; 
            }

            foreach(ASNode neighbour in grid.GetNodeNeighbors(currentNode))
            {
                if (!neighbour.walkable || closedNodes.Contains(neighbour))
                {
                    continue;
                }

                int newGCostToNeighbour = currentNode.gCost + GetManhattanDistance(currentNode, neighbour);

                if (newGCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = newGCostToNeighbour;
                    neighbour.hCost = GetManhattanDistance(neighbour, finishNode);
                    neighbour.parentNode = currentNode;

                    if (!openNodes.Contains(neighbour))
                        openNodes.Add(neighbour);
                }
            }
        }
    }

    int GetManhattanDistance(ASNode nodeA, ASNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return Values.manhattanKD * dstY + Values.manhattanKD * (dstX - dstY);
        return Values.manhattanKD * dstX + Values.manhattanKN * (dstY - dstX);
    }

    void RetracePath(ASNode startNode, ASNode endNode)
    {
        pathNodes = new List<ASNode>();
        ASNode currentNode = endNode;

        while (currentNode != startNode)
        {
            pathNodes.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        pathNodes.Reverse();
        path = pathNodes.Select(node => node.worldPos).ToList();
    }

    private void OnDrawGizmos()
    {
        if (pathNodes!=null)
        {
            Gizmos.color = Color.black;
            foreach(ASNode node in pathNodes)
            {
                Gizmos.DrawWireCube(node.worldPos, new Vector3(grid.GetNodeDiameter, grid.GetNodeDiameter, grid.GetNodeDiameter));
            }
        }
    }
}