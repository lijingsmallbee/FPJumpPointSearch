using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSGameObject : FSObject
{
    private string _name;
    public FSGameObject(string name)
    {
        _name = name;
    }
    private SortedDictionary<string, BaseFSComponent> allComponents = new SortedDictionary<string, BaseFSComponent>();

    public override void OnStep()
    {
        SortedDictionary<string, BaseFSComponent>.Enumerator it = allComponents.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.OnStep();
        }
    }

    public override void OnPostStep()
    {
        throw new NotImplementedException();
    }

    public override void OnDestroy()
    {
        throw new NotImplementedException();
    }

    public T AddComponent<T>() where T : BaseFSComponent
    {
        T inst = Activator.CreateInstance<T>();
        allComponents.Add(inst.GetName(), inst);
        inst.InternalAwake();
        return inst;
    }
}
