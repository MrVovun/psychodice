using System;
using UnityEngine;

namespace BlackboardSource
{
    public abstract class BlackboardValue : BlackboardElement
    {
        public Mode mode { get; private set; }

        [SerializeField, HideInInspector] Mode _serializedMode; // unity hack to use in property drawers

        public BlackboardValue(Mode mode)
        {
            this._serializedMode = mode;
            this.mode = mode;
        }
    }

    [System.Serializable]
    public class BlackboardValue<T> : BlackboardValue
    {
        event Action<T> onChanged;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (BlackboardManager.IsPlaying)
                {
                    if (blackboard.context == null)
                    {
                        Debug.LogError("You can't modify a BlackboardValue out of scope. Use \n ------ \n using(blackboard.Scope(this)) {\n ... \n} \n ------ \nsyntax.");
                        return;
                    }
                    if (Runtime)
                    {
                        if (mode == Mode.OnlyEditor || blackboard.mode == Mode.OnlyEditor)
                        {
                            Debug.LogError("Value is Readonly at runtime.");
                            return;
                        }
                    }
                    else
                    {
                        if (mode == Mode.OnlyRuntime || blackboard.mode == Mode.OnlyRuntime)
                        {
                            Debug.LogError("Value is Readonly in editor.");
                            return;
                        }
                        Debug.LogError("You can't edit Editor Blackboards at runtime through Code.");
                        return;
                    }
                }

                this._lastValueSet = value;
                this.value = value;
                if (BlackboardManager.IsPlaying)
                    onChanged?.Invoke(value);
            }
        }
        [SerializeField] T value;

        private T _lastValueSet;

        public void AddListener(Action<T> callback)
        {
            if (callback == null)
            {
                Debug.LogError("Callback is null");
                return;
            }

            onChanged += callback;
        }

        public void AddListenerAndUpdate(Action<T> callback)
        {
            AddListener(callback);
            callback(Value);
        }

        public void RemoveListener(Action<T> callback)
        {
            if (callback == null)
            {
                Debug.LogError("Callback is null");
                return;
            }
            onChanged -= callback;
        }

        public override void ObserveEditorChanges()
        {
            if (BlackboardManager.IsPlaying) // only at runtime
            {
                if (mode == Mode.Any || mode == Mode.OnlyRuntime) // only FOR runtime values
                {
                    if (Runtime)
                    {
                        bool shouldInvoke = false;

                        // the only case where the they are not equal is if the value was set bypassing the Value property, editor or reflection.
                        if (((object)_lastValueSet) == null)
                        {
                            if (((object)value) == null)
                                return;
                            else
                            {
                                if (!value.Equals(_lastValueSet))
                                {
                                    _lastValueSet = value;
                                    onChanged?.Invoke(value);
                                }
                            }
                        }
                        else
                        {
                            if (!_lastValueSet.Equals(value))
                            {
                                _lastValueSet = value;
                                onChanged?.Invoke(value);
                            }
                        }
                    }
                }
            }
        }

        public BlackboardValue(Mode mode) : base(mode) { }
        public BlackboardValue() : base(Mode.Any) { }
    }
}