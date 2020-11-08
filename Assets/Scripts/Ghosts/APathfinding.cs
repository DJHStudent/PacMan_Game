using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class APathfinding : MonoBehaviour
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
  //  int startX, startY, targetX, targetY;
    int distToNode = 10;
    //////////public APathfinding(int startX, int startY, int targetX, int targetY)
    //////////{
    //////////    this.startX = startX;
    //////////    this.startY = startY;
    //////////    this.targetX = targetX;
    //////////    this.targetY = targetY;
    //////////    pathFinder();
    //////////}


    List<Node> openNodes = new List<Node>();
    List<Node> closedNodes = new List<Node>();
    Node[,] map;
    Node current;
    Vector2 lastDirection;
    Node lowest()
    {
        Node current = openNodes[0];
        foreach (Node node in openNodes)
        {
            if (node.fCost() < current.fCost())
                current = node;
        }
        return current;
    }//way code works the target for ghost 4 is always unreachable so need determine way around(likely through using distances from block)
    public Node pathFinder(int startX, int startY, int targetX, int targetY, Vector2 currDir)
    {
        openNodes.Clear();
        closedNodes.Clear();
        map = GameManager.randomMap.map;
        map[startX, startY].gCost = 0;
        map[startX, startY].hCost = targetDist(startX, startY, targetX, targetY);
        map[startX, startY].parentNode = map[startX, startY];


        openNodes.Add(map[startX, startY]);//new Node(0, , startX, startY, startX, startY));
        while (openNodes.Count > 0)
        {
            Node lastCurrent = current;
            current = lowest();//dont I need to pick one with lowest fcost
            ////////////////////////////////if(lastCurrent != null)
            ////////////////////////////////    lastDirection = (new Vector2(current.currentX, current.currentY) - new Vector2(lastCurrent.currentX, lastCurrent.currentY)).normalized;
            openNodes.Remove(current);
            closedNodes.Add(current);
            if (current.currentX == targetX && current.currentY == targetY)// || Vector2.Distance(new Vector2(targetX, targetY), new Vector2(current.currentX, current.currentY)) <= 2.05f)
            {
                return recalcPath(startX, startY, targetX, targetY, currDir);
            }
            List<Node> neighbourNodes = new List<Node>();//add the nodes from the map with new values
            if (current.currentX + 1 < GameManager.randomMap.width && !(closedNodes.Count == 1 && currDir == Vector2.left))//if tile involves backtracking don't move their
                neighbourNodes.Add(map[current.currentX + 1, current.currentY]);
            if(current.currentX - 1 >= 0 && !(closedNodes.Count == 1 && currDir == Vector2.right))
                neighbourNodes.Add(map[current.currentX - 1, current.currentY]);
            if (current.currentY + 1 < GameManager.randomMap.height && !(closedNodes.Count == 1 && currDir == Vector2.down))
                neighbourNodes.Add(map[current.currentX, current.currentY + 1]);
            if (current.currentY - 1 >= 0 && !(closedNodes.Count == 1 && currDir == Vector2.up))
                neighbourNodes.Add(map[current.currentX, current.currentY - 1]);

            //////////add Right
            ////////neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX + 1, current.currentY), current.currentX, current.currentY, current.currentX + 1, current.currentY));
            //////////add left
            //////////// neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX - 1, current.currentY), current.currentX, current.currentY, current.currentX - 1, current.currentY));
            //////////add Up
            ////////neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX, current.currentY + 1), current.currentX, current.currentY, current.currentX, current.currentY + 1));
            //////////add Down
            ////////neighbourNodes.Add(new Node(current.gCost + distToNode, targetDist(current.currentX, current.currentY - 1), current.currentX, current.currentY, current.currentX, current.currentY - 1));

            foreach (Node currNeighbour in neighbourNodes)
            {
                Vector2 currDirToNode = (new Vector2(currNeighbour.currentX, currNeighbour.currentY) - new Vector2(current.currentX, current.currentY)).normalized;
                if (!validPath(currNeighbour.currentX, currNeighbour.currentY) || closedNodes.Contains(currNeighbour)) //need put in here checking for backtracking
                {
                    continue; //if node already checked or not valid go to next node
                }

                int newMovementCostNeighbour = current.gCost + distToNode;
                if (!openNodes.Contains(currNeighbour) || newMovementCostNeighbour < currNeighbour.gCost)
                {
                    currNeighbour.gCost = newMovementCostNeighbour;
                    currNeighbour.hCost = targetDist(currNeighbour.currentX, currNeighbour.currentY, targetX, targetY);
                    currNeighbour.parentNode = current;
                   //  currNeighbour.dirToNode = (new Vector2(currNeighbour.currentX, currNeighbour.currentY) - new Vector2(current.currentX, current.currentY)).normalized;

                    if (!openNodes.Contains(currNeighbour))//get new path to nebour somehow
                    {
                        openNodes.Add(currNeighbour);
                    }
                   // map[currNeighbour.currentX, currNeighbour.currentY] = currNeighbour;
                }

            }            
        }return null;//no path found
    }
    Node recalcPath(int startX, int startY, int targetX, int targetY, Vector2 currDir)
    {
        Node current = map[targetX, targetY];
        List<Node> path = new List<Node>();
        while (current != map[startX, startY])
        {
            path.Add(current);
            current = current.parentNode;
        }
        if (path.Count > 0)
        {
           // if (newDir != -currDir)
                return path[path.Count - 1];//issue if trying it and on the target path already
           // else
             //   return null;
        }
        else
            return null;
    }

    bool inList(Node node, List<Node> nodeList)
    {
        foreach (Node nodeitem in nodeList)
        {
            if (nodeitem.currentX == node.currentX && nodeitem.currentY == node.currentY)
                return true;
        }
        return false;
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
