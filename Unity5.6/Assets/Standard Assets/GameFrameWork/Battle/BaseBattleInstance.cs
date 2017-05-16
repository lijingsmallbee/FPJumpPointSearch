using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBattleInstance:FSBattleInstance
{
    static private BaseBattleInstance m_inst;
    private FrameController _frameController = new FrameController();
    public static BaseBattleInstance Instance
    {
        get { return m_inst; }
    }
    public BaseBattleInstance()
    {
        if (m_inst == null)
        {
            m_inst = this;
        }
        else
        {
            LogManager.LogError("battle instance want to create more than one instance");
        }
    }

    public override void OnStep()
    {
        _frameController.OnStep();
    }

    public override void OnPostStep()
    {
        _frameController.OnPostStep();
    }

    public override void OnDestroy()
    {
        _frameController.OnDestroy();
    }
    #region all the mananger
    public FrameController frameController
    {
        get { return _frameController; }
    }
    #endregion
    #region public functions
    public void Start()
    {
        _frameController.Reset();
    }
#endregion
}
