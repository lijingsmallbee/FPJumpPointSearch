using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbsComponent : AbsObject
{
    public abstract string GetName();
    public abstract void OnInit();
    public abstract void OnStep();
    public abstract void OnPostStep();
    public abstract void OnDestroy();
}
