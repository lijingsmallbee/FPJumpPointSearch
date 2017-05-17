using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace FSM
{
	enum ParamType
	{
		PT_char,
		PT_int,
		PT_double,
		PT_vec2,
		PT_vec3,
		PT_bool,
		PT_float,
        PT_long,
	}
	[StructLayout(LayoutKind.Explicit)]
	struct ParamValue
	{
		[FieldOffset(0)]
		public char _cval;

		[FieldOffset(0)]
		public int _ival;

		[FieldOffset(0)]
		public double _dval;

		[FieldOffset(0)]
		public Vector2 _vec2val;

		[FieldOffset(0)]
		public Vector3 _vec3val;

		[FieldOffset(0)]
		public bool _bval;

		[FieldOffset(0)]
		public float _fval;
        [FieldOffset(0)]
        public long _lval;
    }

	public class StateParam
	{
		ParamType ptType;
		ParamValue Value;

		public StateParam(char cValue)
		{
			ptType = ParamType.PT_char;
			Value._cval = cValue;
		}

		public StateParam(int iValue)
		{
			ptType = ParamType.PT_int;
			Value._ival = iValue;
		}

		public StateParam(double dValue)
		{
			ptType = ParamType.PT_double;
			Value._dval = dValue;
		}

		public StateParam(Vector2 vValue)
		{
			ptType = ParamType.PT_vec2;
			Value._vec2val = vValue;
		}

		public StateParam(Vector3 vValue)
		{
			ptType = ParamType.PT_vec3;
			Value._vec3val = vValue;
		}

		public StateParam(bool bValue)
		{
			ptType = ParamType.PT_bool;
			Value._bval = bValue;
		}

		public StateParam(float fValue)
		{
			ptType = ParamType.PT_float;
			Value._fval = fValue;
		}

        public StateParam(long lValue)
        {
            ptType = ParamType.PT_long;
            Value._lval = lValue;
        }

		public void SetValue(char value)
		{
			if(ptType == ParamType.PT_char)
			{
				Value._cval = value;
			}
		}

		public void SetValue(int value)
		{
			if(ptType == ParamType.PT_int)
			{
				Value._ival = value;
			}
		}

		public void SetValue(double value)
		{
			if(ptType == ParamType.PT_double)
			{
				Value._dval = value;
			}
		}

		public void SetValue(Vector2 value)
		{
			if(ptType == ParamType.PT_vec2)
			{
				Value._vec2val = value;
			}
		}

		public void SetValue(Vector3 value)
		{
			if(ptType == ParamType.PT_vec3)
			{
				Value._vec3val = value;
			}
		}

		public void SetValue(float value)
		{
			if(ptType == ParamType.PT_float)
			{
				Value._fval = value;
			}
		}

		public void SetValue(bool value)
		{
			if(ptType == ParamType.PT_bool)
			{
				Value._bval = value;
			}
		}

        public void SetValue(long value)
        {
            if (ptType == ParamType.PT_long)
            {
                Value._lval = value;
            }
        }

        public void GetValue(out char value)
		{
			value = Value._cval;
		}

		public void GetValue(out int value)
		{
			value = Value._ival;
		}

		public void GetValue(out Vector2 value)
		{
			value = Value._vec2val;
		}

		public void GetValue(out Vector3 value)
		{
			value = Value._vec3val;
		}

		public void GetValue(out float value)
		{
			value = Value._fval;
		}

		public void GetDouble(out double value)
		{
			value = Value._dval;
		}
		public void GetValue(out bool value)
		{
			value = Value._bval;
		}

        public void GetValue(out long value)
        {
            value = Value._lval;
        }
    }

    public class StateParamDic
    {
        private Dictionary<string, StateParam> m_StateParam = new Dictionary<string, StateParam>();

        public StateParam GetParam(string paramName)
        {
            return m_StateParam[paramName];
        }

        public void SetBool(string name, bool value)
        {
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.SetValue(value);
            }
            else
            {
                m_StateParam.Add(name, new StateParam(value));
            }
        }

        public void GetBool(string name, out bool value)
        {
            value = false;
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.GetValue(out value);
            }
        }

        public void SetVector2(string name, Vector2 value)
        {
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.SetValue(value);
            }
            else
            {
                m_StateParam.Add(name, new StateParam(value));
            }
        }

        public void GetVector2(string name, out Vector2 value)
        {
            value = Vector2.zero;
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.GetValue(out value);
            }
        }

        public void SetVector3(string name, Vector3 value)
        {
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.SetValue(value);
            }
            else
            {
                m_StateParam.Add(name, new StateParam(value));
            }
        }

        public void GetVector3(string name, out Vector3 value)
        {
            value = Vector3.zero;
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.GetValue(out value);
            }
        }

        public void SetInt(string name, int value)
        {
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.SetValue(value);
            }
            else
            {
                m_StateParam.Add(name, new StateParam(value));
            }
        }

        public void GetInt(string name, out int value)
        {
            value = -1;
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.GetValue(out value);
            }
        }

        public void SetFloat(string name, float value)
        {
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.SetValue(value);
            }
            else
            {
                m_StateParam.Add(name, new StateParam(value));
            }
        }

        public void GetFloat(string name, out float value)
        {
            value = 0f;
            StateParam param = null;
            if (m_StateParam.TryGetValue(name, out param))  //find the param
            {
                param.GetValue(out value);
            }
        }
    }
}
