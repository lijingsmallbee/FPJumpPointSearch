using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseBattleInstance : FSGameObject.InternalBattleInstance
{
    private static BaseBattleInstance m_inst = null;
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
    public override void Init()
    {
    //    throw new NotImplementedException();
    }
}
