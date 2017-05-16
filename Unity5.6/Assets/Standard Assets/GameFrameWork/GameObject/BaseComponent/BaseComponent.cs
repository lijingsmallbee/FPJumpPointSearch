using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseFSComponent : FSComponent
{
    enum eComponentStatus
    {
        none,
        awake,
        start,
        step,
    }
    private eComponentStatus _status = eComponentStatus.none;
    private long _frameId = 0;

    public BaseFSComponent()
    {
        _ownerObject = null;
    }
    private FSGameObject _ownerObject;
    public FSGameObject GameObject
    {
        get { return _ownerObject; }
        //这个函数只有第一次调用才有效，后续无法修改
        set { if (_ownerObject == null)
            {
                _ownerObject = value;
            }
        }
    }

    public sealed override void InternalStep()
    {
        if (_status == eComponentStatus.step)
        {
            OnStep();
        }
        else if (_status == eComponentStatus.start && _frameId != BaseBattleInstance.Instance.frameController.GetFrame())
        {
            _status = eComponentStatus.step;
            OnStep();
        }
        else if (_status == eComponentStatus.awake && _frameId != BaseBattleInstance.Instance.frameController.GetFrame())
        {
            _status = eComponentStatus.start;
            _frameId = BaseBattleInstance.Instance.frameController.GetFrame();
            OnStart();
        }
    }

    public sealed override void InternalAwake()
    {
        _frameId = BaseBattleInstance.Instance.frameController.GetFrame();
        _status = eComponentStatus.awake;
        OnAwake();
    }

}
