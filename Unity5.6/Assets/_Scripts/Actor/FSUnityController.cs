using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSUnityController : FSEngineController
{
    private GameObject _unityGameObject;
    public override void OnAwake()
    {
        base.OnAwake();
        _unityGameObject = new GameObject(fsGameObject.Name);
    }

    public override void OnStart()
    {
        base.OnStart();
        string path = "Actors/" + fsGameObject.Name + "/" + fsGameObject.Name;
        UnityEngine.Object obj = Resources.Load(path);
        GameObject inst = GameObject.Instantiate(obj) as GameObject;
        inst.transform.parent = _unityGameObject.transform;
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
    }
}
