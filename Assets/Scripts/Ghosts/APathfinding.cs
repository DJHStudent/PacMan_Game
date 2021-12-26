using System.Collections.Generic;
using UnityEngine;

//approx issue is 106kb used
public class APathfinding
{
    int distToNode = 10;
    List<Node> openNodes = new List<Node>();

    List<Node> usedNodes = new List<Node>();
    public List<Node> path = new List<Node>();
    Node[,] map;
    Node current;

    int currentTargetX = 0, currentTargetY = 0, ghostDistToTarget;
    int validDistToCheck = 40, closedCount = 0;
    Node lowest()
    {
        Node current = openNodes[0];
        foreach (Node node in openNodes)
        {
            if (node.fCost < current.fCost)
                current = node;
        }
        return current;
    }

    void newMap()
    {
        map = new Node[GameManager.randomMap.width, GameManager.randomMap.height];
        Node[,] oldMap = GameManager.randomMap.map;

        for (int i = 0; i < GameManager.randomMap.width; i++)
        {
            for (int j = 0; j < GameManager.randomMap.height; j++)
            {
                map[i, j] = new Node(i, j);
                map[i, j].tileType = oldMap[i, j].tileType;
            }
        }
    }

    public APathfinding()
    {
        newMap();
    }

    public Node pathFinder(int startX, int startY, int targetX, int targetY, Vector2 currDir)
    {
        if (currentTargetX != targetX || currentTargetY != targetY || path.Count == 0)
        {
            openNodes.Clear();
            closedCount = 0;
            path.Clear();
            foreach (Node node in usedNodes)
            {
                node.isClosed = false;
                node.isOpen = false;
            }
            usedNodes.Clear();
            ghostDistToTarget = Mathf.Abs(targetX - startX) + Mathf.Abs(targetY - startY);
            currentTargetX = targetX; currentTargetY = targetY;//if target changes from what currently know need update with new path
            map[startX, startY].gCost = 0;
            map[startX, startY].hCost = targetDist(startX, startY, targetX, targetY);
            map[startX, startY].fCostSet();
            map[startX, startY].parentNode = map[startX, startY];

            int pathsChecked = 0;
            openNodes.Add(map[startX, startY]);
            map[startX, startY].isOpen = true;
            usedNodes.Add(map[startX, startY]);
            while (openNodes.Count > 0)
            {
                current = lowest();
                openNodes.Remove(current);
                closedCount++;
                current.isClosed = true; current.isOpen = false;
                int currentDistToTarget = Mathf.Abs(current.currentX - targetX) + Mathf.Abs(current.currentY - targetY);
                pathsChecked++;
                if (current.currentX == targetX && current.currentY == targetY)
                {
                    return recalcPath(startX, startY, targetX, targetY);
                }
                else if (currentDistToTarget <= ghostDistToTarget - validDistToCheck)
                {
                    return recalcPath(startX, startY, current.currentX, current.currentY);
                }
                List<Node> neighbourNodes = new List<Node>();//add the nodes from the map with new values
                if (current.currentX + 1 < GameManager.randomMap.width && !(closedCount == 1 && currDir == Vector2.left))//if first next tile involves backtracking don't move
                    neighbourNodes.Add(map[current.currentX + 1, current.currentY]);
                if (current.currentX - 1 >= 0 && !(closedCount == 1 && currDir == Vector2.right))
                    neighbourNodes.Add(map[current.currentX - 1, current.currentY]);
                if (current.currentY + 1 < GameManager.randomMap.height && !(closedCount == 1 && currDir == Vector2.down))
                    neighbourNodes.Add(map[current.currentX, current.currentY + 1]);
                if (current.currentY - 1 >= 0 && !(closedCount == 1 && currDir == Vector2.up))
                    neighbourNodes.Add(map[current.currentX, current.currentY - 1]);

                foreach (Node currNeighbour in neighbourNodes)
                {
                    if (!validPath(currNeighbour.currentX, currNeighbour.currentY) || currNeighbour.isClosed)
                    {
                        continue; //if node already checked or not valid go to next node
                    }

                    int newMovementCostNeighbour = current.gCost + distToNode;
                    if (newMovementCostNeighbour < currNeighbour.gCost || !currNeighbour.isOpen)
                    {
                        currNeighbour.gCost = newMovementCostNeighbour;
                        currNeighbour.hCost = targetDist(currNeighbour.currentX, currNeighbour.currentY, targetX, targetY);
                        currNeighbour.fCostSet();
                        currNeighbour.parentNode = current;

                        if (!currNeighbour.isOpen)
                        {
                            openNodes.Add(currNeighbour);
                            currNeighbour.isOpen = true;
                            usedNodes.Add(currNeighbour);
                        }
                    }

                }
            }
            return null;//no path found
        }
        else if (path.Count > 1)
        {
            path.RemoveAt(path.Count - 1);
            return path[path.Count - 1];
        }
        path.Clear();
        return null;
    }
    Node recalcPath(int startX, int startY, int targetX, int targetY)
    {
        Node current = map[targetX, targetY];
        while (current != map[startX, startY])
        {
            path.Add(current);
            current = current.parentNode;
        }
        if (path.Count > 0)
        {
            return path[path.Count - 1];
        }
        else
            return null;
    }
    bool validPath(int currX, int currY)
    {
        return map[currX, currY].tileType == 1 || map[currX, currY].tileType == 11 || map[currX, currY].tileType == 4;
    }
    int targetDist(int currentX, int currentY, int targetX, int targetY)
    {
        int xDist = Mathf.Abs(targetX - currentX);
        int yDist = Mathf.Abs(targetY - currentY);
        return xDist + yDist;
    }
}