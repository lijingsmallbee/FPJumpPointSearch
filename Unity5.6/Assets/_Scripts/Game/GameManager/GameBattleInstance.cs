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
        
    }
    public override void Init()
    {
        TextAsset asset = Resources.Load("Levels/GridInfo") as TextAsset;
        _gridManager.InitGrid(asset.text, eGridType.one);

        asset = Resources.Load("Levels/GridInfo4") as TextAsset;
        _gridManager.InitGrid(asset.text, eGridType.four);
    }
}
