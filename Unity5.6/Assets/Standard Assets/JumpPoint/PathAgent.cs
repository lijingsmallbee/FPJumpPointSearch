using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using TrueSync;
public enum eAgentType
{
    none,
    ground,//地面
    flying,//空中
}
//地面阻挡类型，
public enum eGridGroundBlockType
{
    none,
    thin, //哥布林
    middle, //女武神
    kinght, //骑士,野猪，黑暗王子，王子
    strong,//大胖子
    verystrong,//雷暴车
    construction,//建筑
}

public enum eGridAirBlockType
{
    none,
    thin, //苍蝇
    middle,//飞龙
    strong,//天狗

}

public enum eGridType
{
    none,
    one,
    four,
}
public class MoveAgent
{
    private eAgentType _agentType;
    private FP raidus;
    private FP speed;
    private eGridType _useeGridType;
    private eGridGroundBlockType groundBlockType;
    private eGridAirBlockType airBlockType;
    private bool _ignoreBlock;
    public int GroundBlockValue
    {
        get { return (int)groundBlockType; }
        set
        {
            int real = Mathf.Clamp(value, (int)eGridGroundBlockType.none, (int)eGridGroundBlockType.construction);
            groundBlockType = (eGridGroundBlockType)real;
        }
    }

    public int AirBlockValue
    {
        get { return (int)airBlockType; }
        set
        {
            int real = Mathf.Clamp(value, (int)eGridAirBlockType.none, (int)eGridAirBlockType.strong);
            airBlockType = (eGridAirBlockType)real;
        }
    }

    public bool IgnoreBlock
    {
        get { return _ignoreBlock; }
        set { _ignoreBlock = value; }
    }

    public eAgentType AgentType
    {
        get { return _agentType; }
        set { _agentType = value; }
    }
}