using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int gCost, hCost;
    public int parentX, parentY, currentX, currentY;

    public Node(int gCost, int hCost, int parentX, int parentY, int currentX, int currentY)
    {
        this.gCost = gCost;
        this.hCost = hCost;
        this.parentX = parentX;
        this.parentY = parentY;
        this.currentX = currentX;
        this.currentY = currentY;
    }

    public int fCost()
    {
        return gCost + hCost;
    }
}
