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
    //这个是否可以移动是相对的，强壮的单位会无视比自己弱小的单位
    //还有就是热气球，野猪等直奔对方建筑类型的个体，也是无视碰撞的
    public new bool IsWalkable(int x, int y)
    {
        bool canMove = false;
        eAgentType aType = _agent.AgentType;
        if (aType == eAgentType.ground)
        {
            if(_agent.IgnoreBlock == true)
            {
                canMove = true;
            }
            else if (_agent.GroundBlockValue > _grid[x, y].curGroundValue)
            {
                canMove = true;
            }
        }
        else if (aType == eAgentType.flying)
        {
            if (_agent.AirBlockValue > _grid[x, y].curAirValue)
            {
                canMove = true;
            }
        }
        return (x >= 0 && x < _nodeAmountX) && (y >= 0 && y < _nodeAmountY) && canMove;
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
            JumpFindPathWithGrid findPath = new JumpFindPathWithGrid();
            findPath.Initialize(this);
            _path = findPath;
            //read grid file info
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
            //read file
            CreateGrid(_nodeAmountX, _nodeAmountY);
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

    #region private functions
    private void CreateGrid(int width,int height)
    {
        _nodeAmountX = width;
        _nodeAmountY = height;
        _grid = new Node[_nodeAmountX, _nodeAmountY];
        for(int i=0;i<height;++i)
        {
            for(int j=0;j<width;++j)
            {
                TSVector worldPos = _transformPosition + new TSVector(_nodeUnitSize * ((FP)j + FP.Half), FP.Zero, _nodeUnitSize * ((FP)i + FP.Half));
                Node newNode = new Node(worldPos,j,i,_nodeUnitSize);
                _grid[i,j] = newNode;
            }
        }
    }
#endregion
}
