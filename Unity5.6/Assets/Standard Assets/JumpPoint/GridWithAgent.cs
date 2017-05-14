using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
public class GridWithAgent:Grid
{
    protected MoveAgent _agent;
    public void Initialize(Transform gridTransform, LayerMask unwalkableLayerMask, FP nodeUnitSize, int nodeAmountX, int nodeAmountY)
    {
        _transformPosition = new TSVector(gridTransform.position.x, gridTransform.position.y, gridTransform.position.z);
        _transformScale = new TSVector(gridTransform.localScale.x,gridTransform.localScale.y,gridTransform.localScale.z);
        _unwalkableLayerMask = unwalkableLayerMask;
        _nodeUnitSize = nodeUnitSize;
        _nodeAmountX = nodeAmountX;
        _nodeAmountY = nodeAmountY;

        _grid = new Node[_nodeAmountX, _nodeAmountY];
        _path = new FindPath();
    }

    public List<Node> GetPath(TSVector2 startPosition, TSVector2 targetPosition,MoveAgent agent)
    {
        _agent = agent;
        Node startNode = GetNodeFromPoint((int)startPosition.x, (int)startPosition.y);
        Node targetNode = GetNodeFromPoint((int)targetPosition.x, (int)targetPosition.y);
        return _path.GetPath(startNode, targetNode);
    }

    protected Node GetNodeFromPoint(TSVector2 pos)
    {
        TSVector2 localPos = pos - new TSVector2(_transformPosition.x,_transformPosition.z);

        int x = (int)TSMath.Round(localPos.x / _nodeUnitSize);
        int y = (int)TSMath.Round(localPos.y / _nodeUnitSize);

        x = Mathf.Clamp(x, 0, _nodeAmountX - 1);
        y = Mathf.Clamp(y, 0, _nodeAmountY - 1);

        return _grid[x, y];
    }

    public new bool IsWalkable(int x, int y)
    {
        bool canmove = _agent.BlockValue > _grid[x, y].currentValue;
        bool canmove2 = _agent.IgnoreBlock || (!_agent.IgnoreBlock && _grid[x, y].originalValue == 0);
        canmove = canmove && canmove2;
        return (x >= 0 && x < _nodeAmountX) && (y >= 0 && y < _nodeAmountY) && canmove;
    }

    public void CreateFromString(string buffer)
    {
        int start = 0;
        string line = TextUtils.readLine(buffer, ref start);
        List<string> words = TextUtils.splitLine(line);
        bool isRightMap = false;
        if (words.Count == 2)
        {
            if(words[0] == "type" && words[1] == "octile")
            {
                isRightMap = true;
            }
        }
        if(isRightMap)
        {
            _grid = new Node[_nodeAmountX, _nodeAmountY];
            JumpFindPathWithGrid findPath = new JumpFindPathWithGrid();
            findPath.Initialize(this);
            _path = findPath;
            string offsetx = TextUtils.readLine(buffer, ref start);
            words = TextUtils.splitLine(offsetx);
            FP offsetX = FP.FromRaw(long.Parse(words[1]));
            string offsety = TextUtils.readLine(buffer, ref start);
            words = TextUtils.splitLine(offsety);
            FP offsetY = FP.FromRaw(long.Parse(words[1]));
            _transformPosition = new TSVector(offsetX, 0f, offsetY);
            string width = TextUtils.readLine(buffer, ref start);
            words = TextUtils.splitLine(width);
            _nodeAmountX = int.Parse(words[1]);
            string height = TextUtils.readLine(buffer, ref start);
            words = TextUtils.splitLine(height);
            _nodeAmountY = int.Parse(words[1]);
            string size = TextUtils.readLine(buffer, ref start);
            words = TextUtils.splitLine(size);
            _nodeUnitSize = FP.FromRaw(long.Parse(words[1]));
            string nouseTag = TextUtils.readLine(buffer, ref start);
            
            for (int i = 0; i < _nodeAmountY; ++i)
            {
                string blocks = TextUtils.readLine(buffer, ref start);
                for(int j=0;j<_nodeAmountX;++j)
                {
                    char v = blocks[j];
                    _grid[i, j].originalValue = v == '@' ? 1 : 0;
                }
            }
        }
    }
}
