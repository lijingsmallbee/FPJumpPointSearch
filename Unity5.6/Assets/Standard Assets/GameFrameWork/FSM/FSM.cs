using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace FSM
{
    public enum ChangeResult
    {
        SameState,
        Success,
        error,
        //the enum below is the error reason
        //judge if sucess can use ret > error
        NotExist,
        ParamError,
        NotInit,
    }
    //	public delegate void OnStateChange(ActorStateType stateType);
    /// Control all actor states.
    /// 

    public delegate void StateChange(string stateName);
    public class BaseFSM
    {
        private GameObject m_owner;
        private Dictionary<string, BaseState> m_StateMap = new Dictionary<string, BaseState>();
        StateParamDic m_stateParam = null; 
        private BaseState m_curState = null;

        public event StateChange m_stateEvent;

        public void InitAllState()
        {
            Dictionary<string, BaseState>.Enumerator enu = m_StateMap.GetEnumerator();
            while (enu.MoveNext())
            {
                enu.Current.Value.OnInitState();
            }
        }

        #region Public Methods

        #region constructor
        public BaseFSM(GameObject owner,FSM.StateParamDic dic)
        {
            m_owner = owner;
            if(dic == null)
            {
                m_stateParam = new StateParamDic();
            }
        }
        #endregion
        #region public properties
        public GameObject OwnerObject
        {
            get { return m_owner; }
        }
        #endregion
        public BaseState GetState(string name)
        {
            BaseState ret = null;
            m_StateMap.TryGetValue(name, out ret);
            return ret;
        }

        public BaseState CurState
        {
            get { return m_curState; }
            //not allow set current state
        //    set { m_curState = value; }
        }

        public string GetCurStateName()
        {
            string ret = string.Empty;
            if (CurState)
            {
                ret = CurState.GetName();
            }
            return ret;
        }

        public void Reset()
        {
            if(CurState)
            {
                CurState.OnLeaveState();
            }
            m_curState = null;
        }

        public void Update()
        {
            if(CurState)
            {
                CurState.OnUpdateState();
            }
        }

        public bool DebugLog
        {
            get;
            set;
        }

        public void HandleEvent(StateEventType evt)
        {
            if(m_curState != null)
            {
                m_curState.HandleEvent(evt);
            }
        }

        public ChangeResult ChangeTo(string stateName)
        {
            BaseState enterState = null;
            if (!m_StateMap.TryGetValue(stateName, out enterState))
            {
                return ChangeResult.NotExist;
            }
            if (m_curState != null && m_curState.GetName() == stateName)
            {
                CurState.OnReEnter();
                return ChangeResult.SameState;
            }
            else  //enter new state
            {
                if (DebugLog)
                {
                    LogManager.LogError("current state is " + CurState.GetName() + "chang to " + stateName);
                }
                if (CurState)
                {
                    CurState.OnLeaveState();
                }
                m_curState = enterState;
                CurState.OnEnterState();
                if (m_stateEvent != null)
                {
                    m_stateEvent(stateName);
                }
                return ChangeResult.Success;
            }
        }

        public void AddState(BaseState state)
        {
            if(!m_StateMap.ContainsKey(state.GetName()))
            {
                m_StateMap.Add(state.GetName(), state);
            }
            else
            {
                LogManager.LogError("add same state named " + state.GetName());
            }
        }

        void AddState<T>() where T : BaseState
        {
            T inst = (T)System.Activator.CreateInstance(typeof(T));
            if (m_StateMap.ContainsKey(inst.GetName()))
            {
                LogManager.LogError("Add same state" + inst.GetName());
                m_StateMap.Remove(inst.GetName());
            }
            m_StateMap.Add(inst.GetName(), inst);
        }

    /*    public void AddScriptState(string className, bool useMove)
        {
            ScriptState state = new ScriptState();
            state.className = className;
            state.useMove = useMove;
            if (m_StateMap.ContainsKey(state.GetStateName()))
            {
                LogManager.LogError("Add same state" + state.GetStateName());
                m_StateMap.Remove(state.GetStateName());
            }
            m_StateMap.Add(state.GetStateName(), state);
        } */
        #endregion
        #region internel function

        #endregion
    }
}
