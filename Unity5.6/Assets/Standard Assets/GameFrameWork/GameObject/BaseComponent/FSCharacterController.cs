using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
public class FSCharacterController : BaseFSComponent
{
    FP _radisu;
    MoveAgent _agent = null;

    public override void OnAwake()
    {
        _agent = new MoveAgent();
    }

    public override void OnStart()
    {
        throw new NotImplementedException();
    }

    public override string GetName()
    {
        return typeof(FSCharacterController).Name;
    }

    public override void OnStep()
    {
        throw new NotImplementedException();
    }

    public override void OnPostStep()
    {
        throw new NotImplementedException();
    }

    public override void OnDestroy()
    {
        throw new NotImplementedException();
    }
}
