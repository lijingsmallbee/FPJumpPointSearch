using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController:FSObject
{
    private long _frameId;
    public long GetFrame()
    {
        return _frameId;
    }

    public override void OnStep()
    {
        _frameId++;
    }

    public override void OnPostStep()
    {
        //do nothing
    }

    public override void OnDestroy()
    {
        
    }

    public void Reset()

    {
        _frameId = 0;
    }
}
