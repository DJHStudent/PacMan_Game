using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APathfinding
{
    //get distance to next location, will always retrun 1 as can never actually go diagronally
    //distance can be manhattan distance of xDist + yDist
    //in start position looks at all avaliable positions to look at in one tile away and chooses node with lowest fCost //for mine only need check 4 directions
    //once node checked marked and put in the closed list //then recalculates all stuff again from current position

    //if two+ nodes have same fCost use one with lowest hCost

    //for each node also have pointer to the node it came from as well

    //when calculating nodes also have single list of nebours

    //in node class
    //has gCost = distance from starting node //when calculating this also note that it takes in the node it came fromes gCost and adds its distance from node checking onto it
    //hCost = distance from end node
    //fCost = gCost + hCost
    //has parent which is node came from
    int startX, startY, targetX, targetY;
    int distToNode = 10;
    public APathfinding(int startX, int startY, int targetX, int targetY)
    {
        this.startX = startX;
        this.startY = startY;
        this.targetX = targetX;
        this.targetY = targetY;
    }


    List<Node> openNodes = new List<Node>();
    List<Node> closedNodes = new List<Node>();
    int[,] map;

    public void pathFinder()
    {
        map = GameManager.randomMap.map;
        openNodes.Add(new Node(0, targetDist(startX, startY), startX, startY, startX, startY));
        while (openNodes.Count > 0)
        {
            Node current = openNodes[0];
            openNodes.Remove(current);
            closedNodes.Add(current);
            if (current.currentX == targetX && current.currentY == targetY)
                return;
            List<Node> neighbourNodes = new List<Node>();
            //add Right
            neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX + 1, current.currentY), current.currentX, current.currentY, current.currentX + 1, current.currentY));
            //add left
            neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX - 1, current.currentY), current.currentX, current.currentY, current.currentX - 1, current.currentY));
            //add Up
            neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX, current.currentY + 1), current.currentX, current.currentY, current.currentX, current.currentY + 1));
            //add Down
            neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX, current.currentY - 1), current.currentX, current.currentY, current.currentX, current.currentY - 1));

            foreach (Node currNeighbour in neighbourNodes)
            {
                if (!validPath(currNeighbour.currentX, currNeighbour.currentY) || isClosedOrOpen(currNeighbour, closedNodes))
                    continue; //if node already checked or not valid go to next node
                if (!isClosedOrOpen(currNeighbour, openNodes))//get new path to nebour somehow
                {
                    //update gCost
                    if (!isClosedOrOpen(currNeighbour, openNodes))
                        openNodes.Add(currNeighbour);
                }
            }
        }

    }
    bool validPath(int currX, int currY)
    {
        return map[currX, currY] == 1 || map[currX, currY] == 11;
    }

    bool isClosedOrOpen(Node curr, List<Node> nodeList)
    {
        foreach (Node closed in nodeList)
        {
            if (closed == curr)
                return true;
        }
        return false; //node not closed off
    }

    int targetDist(int currentX, int currentY)
    {
        int xDist = Mathf.Abs(targetX - currentX);
        int yDist = Mathf.Abs(targetY - currentY);
        return xDist + yDist;
    }
}
