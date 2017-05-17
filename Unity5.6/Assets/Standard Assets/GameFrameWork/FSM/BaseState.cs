using UnityEngine;
using System.Collections;
namespace FSM
{
    public enum StateEventType
    {
        none,
        moveDir,
        skill,
        moveStop,
        moveToPos,
        moveToTarget,
        moveNav,
        dodge,
        death,
        custom,
        attack,
        damage,
        hitBack,
        hitFly,
        count,
    }
	/// The base class of actor state.
	public abstract class BaseState
	{
		#region Internal Usage
		protected BaseFSM m_FSM;
        #endregion

        #region constructor
        public BaseState(BaseFSM owner)
        {
            m_FSM = owner;
        }
        #endregion
        #region Interface Methods
        public abstract string GetName();
        public abstract void OnInitState();
        public abstract void OnEnterState();
        public abstract void OnReEnter();
        public abstract void OnLeaveState();
        public abstract void OnUpdateState();
        public abstract void HandleEvent(StateEventType eventT);
		#endregion
        #region implement () operator
        public static implicit operator bool (BaseState state) {
            return state != null;
        }
        #endregion
	}
}
