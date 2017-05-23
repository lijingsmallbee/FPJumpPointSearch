using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleInstance : BaseBattleInstance
{
    private GridManager _gridManager = new GridManager();

    public override void Start()
    {
        base.Start();
        TextAsset asset = Resources.Load("Levels/GridInfo") as TextAsset;
        _gridManager.InitGrid(asset.text, eGridType.one);
    }
    public override void Init()
    {
        
    }
}
