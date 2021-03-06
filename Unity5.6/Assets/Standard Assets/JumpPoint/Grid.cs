﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
public class Grid
{
    public bool showDebug;
    public bool AStar;
    #region Singleton
    private static Grid _instance = null;
    public static Grid Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Grid();
            return _instance;
        }
    }
    #endregion

    protected int _nodeAmountX;
    protected int _nodeAmountY;
    public int NodeAmountX
    {
        get
        {
            return _nodeAmountX;
        }
    }
    public int NodeAmountY
    {
        get
        {
            return _nodeAmountY;
        }
    }

    public FP _nodeUnitSize;
    //whitebai 2017.5.9
    //private Transform _gridTransform = null;

    protected TSVector _transformPosition;

    protected TSVector _transformScale = TSVector.one;

    protected LayerMask _unwalkableLayerMask;

    protected FindPath _path = null;


    protected Node[,] _grid = null;
    //temp accessor
    public Node[,] WorldGrid
    {
        get
        {
            return _grid;
        }
    }
    private HashSet<Node> _unwalkableNodes;
    //temp accessor
    public HashSet<Node> UnwalkableNodes
    {
        get
        {
            return _unwalkableNodes;
        }
    }

    public int GridSize
    {
        get
        {
            return _nodeAmountX * _nodeAmountY;
        }
    }

    public List<Node> GetJump()
    {
        return _path.jumpNodes;
    }

    FindAPath path;
    public List<Node> GetClosed()
    {
        return path.closedList;
    }

    public void InitializeGrid(Transform gridTransform, LayerMask unwalkableLayerMask, FP nodeUnitSize, int nodeAmountX, int nodeAmountY)
    {
        //whitebai 2017.5.9
        //_gridTransform = gridTransform;
        _transformPosition = new TSVector(gridTransform.position.x, gridTransform.position.y, gridTransform.position.z);
        _transformScale = new TSVector(gridTransform.localScale.x,gridTransform.localScale.y,gridTransform.localScale.z);
        _unwalkableLayerMask = unwalkableLayerMask;
        _nodeUnitSize = nodeUnitSize;
        _nodeAmountX = nodeAmountX;
        _nodeAmountY = nodeAmountY;

        _unwalkableNodes = new HashSet<Node>();
        _grid = new Node[_nodeAmountX, _nodeAmountY];
        _path = new FindPath();
        path = new FindAPath();

        _CreateGrid();
    }

    //REMOVE - A STAR COMPARISON
    public List<Node> GetAStarNeighbours(Node currentNode)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (IsWalkable(x + currentNode.x, y + currentNode.y))
                {
                    neighbours.Add(_grid[x + currentNode.x, y + currentNode.y]);
                }
            }
        }
        return neighbours;
    }


    public List<Node> GetPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = GetNodeFromPoint((int)startPosition.x, (int)startPosition.z);
        Node targetNode = GetNodeFromPoint((int)targetPosition.x, (int)targetPosition.z);

        if (AStar)
        {
            return path.GetPath(startNode, targetNode);
        }

        return _path.GetPath(startNode, targetNode);
    }

    private void _CreateGrid()
    {
        TSVector xOffset = TSVector.right * (_transformScale.x/2);
        TSVector yOffset = TSVector.forward * (_transformScale.z / 2);
        TSVector worldBottomLeft = _transformPosition - xOffset - yOffset;

        for (int x = 0; x < _nodeAmountX; x++)
        {
            for (int y = 0; y < _nodeAmountY; y++)
            {
                FP xCalculation = x * _nodeUnitSize;
                FP yCalculation = y * _nodeUnitSize;
                TSVector worldPoint = worldBottomLeft + (TSVector.right * xCalculation) + (TSVector.forward * yCalculation);

                RaycastHit hit;
                TSVector pos = worldPoint + TSVector.up * 500;
                bool walkable = !(Physics.SphereCast(new Vector3((float)pos.x,(float)pos.y,(float)pos.z), (float)_nodeUnitSize / 2,
                                    Vector3.down, out hit, Mathf.Infinity, _unwalkableLayerMask));

                _grid[x, y] = new Node(worldPoint, x, y, _nodeUnitSize);

                if (!walkable)
                    _unwalkableNodes.Add(_grid[x, y]);
            }
        }
    }

    public Node GetNodeFromPoint(int nodeX, int nodeY)
    {
        FP percentX = nodeX / (FP)_nodeAmountX;
        FP percentY = nodeY / (FP)_nodeAmountY;

        percentX = TSMath.Clamp(percentX,FP.Zero,FP.One);
        percentY = TSMath.Clamp(percentY, FP.Zero, FP.One);

        int x = (int)TSMath.Round((((_nodeAmountX / _nodeUnitSize)) * percentX));
        int y = (int)TSMath.Round((((_nodeAmountY / _nodeUnitSize)) * percentY));

        x = Mathf.Clamp(x, 0, _nodeAmountX - 1);
        y = Mathf.Clamp(y, 0, _nodeAmountY - 1);
        
        return _grid[x, y];
    }

    public Node GetNodeFromPoint(FP nodeX, FP nodeY)
    {
        FP percentX = nodeX / (FP)_nodeAmountX;
        FP percentY = nodeY / (FP)_nodeAmountY;

        percentX = TSMath.Clamp(percentX, FP.Zero, FP.One);
        percentY = TSMath.Clamp(percentY, FP.Zero, FP.One);

        int x = (int)TSMath.Round((((_nodeAmountX / _nodeUnitSize)) * percentX));
        int y = (int)TSMath.Round((((_nodeAmountY / _nodeUnitSize)) * percentY));

        x = Mathf.Clamp(x, 0, _nodeAmountX - 1);
        y = Mathf.Clamp(y, 0, _nodeAmountY - 1);

        return _grid[x, y];
    }

    public Node GetNodeFromIndex (int x, int y)
    {
        if (!IsWalkable(x, y))
            return null;
        return _grid[x, y];
    }

    public List<Node> GetNeighbours(Node currentNode)
    {
        List<Node> neighbours = new List<Node>();

        Node parentNode = currentNode.parent;

        if (parentNode == null)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (IsWalkable(x + currentNode.x, y + currentNode.y))
                    {
                        neighbours.Add(_grid[x + currentNode.x, y + currentNode.y]);
                    }
                }
            }
        }
        else
        {
            int xDirection = Mathf.Clamp(currentNode.x - parentNode.x, -1, 1);
            int yDirection = Mathf.Clamp(currentNode.y - parentNode.y, -1, 1);

            if (xDirection != 0 && yDirection != 0)
            {
                //assumes positive direction for variable naming
                bool neighbourUp = IsWalkable(currentNode.x, currentNode.y + yDirection);
                bool neighbourRight = IsWalkable(currentNode.x + xDirection, currentNode.y);
                bool neighbourLeft = IsWalkable(currentNode.x - xDirection, currentNode.y);
                bool neighbourDown = IsWalkable(currentNode.x, currentNode.y - yDirection);

                if (neighbourUp)
                    neighbours.Add(_grid[currentNode.x, currentNode.y + yDirection]);

                if (neighbourRight)
                    neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y]);

                if (neighbourUp || neighbourRight)
                    if (IsWalkable(currentNode.x + xDirection, currentNode.y + yDirection))
                        neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y + yDirection]);

                if (!neighbourLeft && neighbourUp)
                    if (IsWalkable(currentNode.x - xDirection, currentNode.y + yDirection))
                        neighbours.Add(_grid[currentNode.x - xDirection, currentNode.y + yDirection]);

                if (!neighbourDown && neighbourRight)
                    if (IsWalkable(currentNode.x + xDirection, currentNode.y - yDirection))
                        neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y - yDirection]);
            }
            else
            {
                if (xDirection == 0)
                {
                    if (IsWalkable(currentNode.x, currentNode.y + yDirection))
                    {
                        neighbours.Add(_grid[currentNode.x, currentNode.y + yDirection]);

                        if (!IsWalkable(currentNode.x + 1, currentNode.y))
                            if (IsWalkable(currentNode.x + 1, currentNode.y + yDirection))
                                neighbours.Add(_grid[currentNode.x + 1, currentNode.y + yDirection]);

                        if (!IsWalkable(currentNode.x - 1, currentNode.y))
                            if (IsWalkable(currentNode.x - 1, currentNode.y + yDirection))
                                neighbours.Add(_grid[currentNode.x - 1, currentNode.y + yDirection]);
                    }
                }
                else
                {
                    if (IsWalkable(currentNode.x + xDirection, currentNode.y))
                    {
                        neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y]);
                        if (!IsWalkable(currentNode.x, currentNode.y + 1))
                            neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y + 1]);
                        if (!IsWalkable(currentNode.x, currentNode.y - 1))
                            neighbours.Add(_grid[currentNode.x + xDirection, currentNode.y - 1]);
                    }
                }
            }
        }
        return neighbours;
    }

    public bool IsWalkable(int x, int y)
    {
        return (x >= 0 && x < _nodeAmountX) && (y >= 0 && y < _nodeAmountY) && !_unwalkableNodes.Contains(_grid[x, y]);
    }
}
