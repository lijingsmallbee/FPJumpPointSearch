using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public sealed class  FSGameObject : object
{
    private string _name;
    private bool _needDestroy = false;
    private FSGameObject(string name)
    {
        _name = name;
    }
    private SortedDictionary<string, BaseFSComponent> allComponents = new SortedDictionary<string, BaseFSComponent>();

    static private MethodInfo internalStep = null;
    static private MethodInfo internalAwake = null;
    static private object[] param = new object[0];
    static private void InitPrivateCall()
    {
        internalStep = typeof(BaseFSComponent).GetMethod("InternalStep", BindingFlags.Instance | BindingFlags.NonPublic);
        internalAwake = typeof(BaseFSComponent).GetMethod("InternalAwake", BindingFlags.Instance | BindingFlags.NonPublic);
    }
    private void Step()
    {
        SortedDictionary<string, BaseFSComponent>.Enumerator it = allComponents.GetEnumerator();
        while (it.MoveNext())
        {
            internalStep.Invoke(it.Current.Value, param);
         //   it.Current.Value.InternalStep();
        }
    }

    private void PostStep()
    {
        SortedDictionary<string, BaseFSComponent>.Enumerator it = allComponents.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.OnPostStep();
        }
    }

    private void OnDestroy()
    {
        SortedDictionary<string, BaseFSComponent>.Enumerator it = allComponents.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.OnDestroy();
        }
    }

    public T AddComponent<T>() where T : BaseFSComponent
    {
        T inst = Activator.CreateInstance<T>();
        allComponents.Add(inst.GetName(), inst);
        inst.fsGameObject = this;
        internalAwake.Invoke(inst, param);
        return inst;
    }
#region public propeties
    public string Name
    {
        get { return _name; }
        set
        {
            if(!string.IsNullOrEmpty(value))
            {
                _name = value;
            }

        }
    }

    private bool NeedDestroy
    {
        get { return _needDestroy; }
        set { _needDestroy = value; }
    }
    #endregion

    #region class BattleInstance
    public abstract class InternalBattleInstance : FSBattleInstance
    {
        
        private FrameController _frameController = new FrameController();
        protected SortedDictionary<string, FSGameObject> gameObjectDictionary = new SortedDictionary<string, FSGameObject>();

        protected InternalBattleInstance()
        {
            FSGameObject.InitPrivateCall();
        }

        public virtual FSGameObject CreateGameObject(string name)
        {
            if (gameObjectDictionary.ContainsKey(name))
            {
                Debug.LogError("already has game object" + name);
                return null;
            }
            FSGameObject inst = new FSGameObject(name);
            gameObjectDictionary.Add(name, inst);
            return inst;
        }

        public virtual void DestroyGameObject(FSGameObject gameObj)
        {
            gameObj.NeedDestroy = true;
        }

        public override void OnStep()
        {
            _frameController.OnStep();
            SortedDictionary<string, FSGameObject>.Enumerator goIt = gameObjectDictionary.GetEnumerator();
            while (goIt.MoveNext())
            {
                goIt.Current.Value.Step();
            }
            //then do post step
            DoPostStep();
        }

        public override void OnPostStep()
        {
            //do nothing
        }

        private void DoPostStep()
        {
            List<string> deleted = new List<string>(128);
            SortedDictionary<string, FSGameObject>.Enumerator goIt = gameObjectDictionary.GetEnumerator();
            while (goIt.MoveNext())
            {
                goIt.Current.Value.PostStep();
                if (goIt.Current.Value.NeedDestroy)
                {
                    deleted.Add(goIt.Current.Key);
                    goIt.Current.Value.OnDestroy();
                }
            }

            List<string>.Enumerator deleteIt = deleted.GetEnumerator();
            while (deleteIt.MoveNext())
            {
                gameObjectDictionary.Remove(deleteIt.Current);
            }
            _frameController.OnPostStep();
        }

        public override void OnDestroy()
        {
            _frameController.OnDestroy();
        }
        #region all the mananger
        public FrameController frameController
        {
            get { return _frameController; }
        }
        #endregion
        #region public functions
        public virtual void Start()
        {
            _frameController.Reset();
        }

        public abstract void Init();
        #endregion
    }
    #endregion
}
