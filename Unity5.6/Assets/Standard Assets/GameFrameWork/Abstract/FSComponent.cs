using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class FSComponent : FSObject
{
    public abstract void OnAwake();
    public abstract void OnStart();
    public abstract string GetName();
}
