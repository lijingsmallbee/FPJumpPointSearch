using System;
using System.Collections.Generic;

namespace TrueSync
{
    public abstract class ResourcePool
    {
        //
        // Static Fields
        //
        protected static List<ResourcePool> resourcePoolReferences;

        //
        // Fields
        //
        protected bool fresh;

        //
        // Constructors
        //
        protected ResourcePool()
        {
        }

        //
        // Static Methods
        //
        public static void CleanUpAll()
        {
            List<ResourcePool>.Enumerator it = resourcePoolReferences.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.ResetResourcePool();
            }
        }

        //
        // Methods
        //
        public abstract void ResetResourcePool();
    }



    public class ResourcePool<T> : ResourcePool
    {
        private int _Count;
        //
        // Fields
        //
        protected Stack<T> stack;

        //
        // Properties
        //
        public int Count
        {
            get { return _Count; }
        }

        //
        // Constructors
        //
        public ResourcePool()
        {
            resourcePoolReferences.Add(this);
        }

        //
        // Methods
        //
        public T GetNew()
        {
            if (_Count > 0)
            {
                _Count--;
                return stack.Pop();
            }
            else
            {
                return NewInstance();
            }
        }

        public void GiveBack(T obj)
        {
            stack.Push(obj);
            _Count++;
        }

        protected virtual T NewInstance()
        {
            return Activator.CreateInstance<T>();
        }

        public override void ResetResourcePool()
        {
            stack.Clear();
            _Count = 0;
        }
    }
}
