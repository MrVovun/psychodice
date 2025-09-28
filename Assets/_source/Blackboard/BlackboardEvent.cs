using System;
using UnityEngine;

namespace BlackboardSource
{
    public abstract class BlackboardEventBase : BlackboardElement
    {
        [SerializeField, HideInInspector]
        private int _unitySerializePleaseHack;

        protected bool RuntimeCheck()
        {
            if (!BlackboardManager.IsPlaying)
            {
                Debug.LogError("Blackboard Events can only be used at runtime, Blackboard Mode is irrelevant.");
                return false;
            }

            if (!Runtime)
            {
                Debug.LogError("Only Runtime copies of BlackboardEvent can be used.");
                return false;
            }

            return true;
        }

        protected bool ScopeCheck()
        {
            if (blackboard.context == null)
            {
                Debug.LogError("You can't call a BlackboardEvent out of scope. Use \n ------ \n using(blackboard.Scope(this)) {\n ... \n} \n ------ \nsyntax.");
                return false;
            }

            return true;
        }

        public abstract void InvokeBoxed(object scope, object arg = null);
    }

    [System.Serializable]
    public class BlackboardEvent : BlackboardEventBase
    {
        event Action onChanged;

        public void AddListener(Action callback)
        {
            if (!RuntimeCheck())
                return;

            onChanged += callback;
        }

        public void RemoveListener(Action callback)
        {
            if (!RuntimeCheck())
                return;

            onChanged -= callback;
        }

        public void Invoke()
        {
            if (!RuntimeCheck() || !ScopeCheck())
                return;
            onChanged?.Invoke();
        }

        /// <summary>
        /// Convenience method to avoid using scope. If in scope, the scope will override the caller param.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="caller">Alternative to scope. Will be overwriten by scope.</param>
        public void Invoke(object scope)
        {
            if (!RuntimeCheck())
                return;

            if (blackboard.context != null)
                scope = blackboard.context;

            if (scope == null)
            {
                Debug.LogError("You can't invoke without a scope, or withough providing a caller param.");
                return;
            }

            onChanged?.Invoke();
        }

        public override void InvokeBoxed(object scope, object arg = null)
        {
            Invoke(scope);
        }
    }

    [System.Serializable]
    public class BlackboardEvent<T> : BlackboardEventBase
    {
        event Action<T> onChanged;

        public void AddListener(Action<T> callback)
        {
            if (!RuntimeCheck())
                return;

            onChanged += callback;
        }

        public void RemoveListener(Action<T> callback)
        {
            if (!RuntimeCheck())
                return;

            onChanged -= callback;
        }

        public void Invoke(T value)
        {
            if (!RuntimeCheck() || !ScopeCheck())
                return;
            onChanged?.Invoke(value);
        }

        /// <summary>
        /// Convenience method to avoid using scope. If in scope, the scope will override the caller param.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="caller">Alternative to scope. Will be overwriten by scope.</param>
        public void Invoke(T value, object caller)
        {
            if (!RuntimeCheck())
                return;

            if (blackboard.context != null)
                caller = blackboard.context;

            if(caller == null)
            {
                Debug.LogError("You can't invoke without a scope, or withough providing a caller param.");
                return;
            }

            onChanged?.Invoke(value);
        }

        public override void InvokeBoxed(object scope, object arg = null)
        {
            Invoke((T)arg, scope);
        }
    }
}