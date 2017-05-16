using UnityEngine;
using System.Collections;
using System;
using TrueSync;
public class Node : IHeapItem<Node>
{
    public Node parent;
    public TSVector worldPosition;
    public FP nodeSize;
    public int x;
    public int y;
    public int curGroundValue;
    public int curAirValue;
    public int originalValue;

    private int _heapIndex = 0;
    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(TSVector worldPoint, int _x, int _y, FP _nodeSize)
    {
        worldPosition = worldPoint;
        nodeSize = _nodeSize;
        x = _x;
        y = _y;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
    public override string ToString()
    {
        return x.ToString() + " , " + y.ToString();
    }
}