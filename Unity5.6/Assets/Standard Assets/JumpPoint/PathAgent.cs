using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using TrueSync;
public enum GridBlockType
{
    none,
    thin,
    middle,
    strong,
    verystrong,
}

public enum GridType
{
    none,
    one,
    four,
}
public class MoveAgent
{
    private FP raidus;
    private FP speed;
    private GridType _useGridType;
    private GridBlockType blockType;
    private bool _ignoreBlock;
    public int BlockValue
    {
        get { return (int)blockType; }
        set
        {
            int real = Mathf.Clamp(value, (int)GridBlockType.none, (int)GridBlockType.verystrong);
            blockType = (GridBlockType)real;
        }
    }

    public bool IgnoreBlock
    {
        get { return _ignoreBlock; }
        set { _ignoreBlock = value; }
    }
}