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
    public FSGameObject fsGameObject
    {
        get { return _ownerObject; }
        //这个函数只有第一次调用才有效，后续无法修改
        set { if (_ownerObject == null)
            {
                _ownerObject = value;
            }
        }
    }

    private void InternalStep()
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

    private void InternalAwake()
    {
        _frameId = BaseBattleInstance.Instance.frameController.GetFrame();
        _status = eComponentStatus.awake;
        OnAwake();
    }

    public sealed override  string GetName()
    {
        return this.GetType().Name;
    }

    public string Name
    {
        get { return fsGameObject.Name; }
        set { fsGameObject.Name = value; }
    }

}
