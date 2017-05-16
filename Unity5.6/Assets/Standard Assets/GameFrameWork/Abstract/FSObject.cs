using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSObject
{
    public abstract void OnStep(); //update
    public abstract void OnPostStep(); //late update
    public abstract void OnDestroy(); //destroy
}
