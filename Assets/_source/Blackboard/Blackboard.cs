namespace BlackboardSource
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;

    public enum Mode
    {
        Any = 0,
        OnlyEditor = 1,
        OnlyRuntime = 2,
    }

    [CreateAssetMenu(fileName = "Blackboard", menuName = "_Dice/Blackboard", order = -1000)]
    public abstract class Blackboard : ScriptableObject, IEditorDirty
    {
        [SerializeField, HideInInspector] Color color = new Color(1,1,1,1);

        public Dictionary<string, BlackboardElement> runtimeElements { get; private set; }
        public bool IsRuntime => isRuntime;
        public Blackboard runtimeCopy => BlackboardManager.Instance.GetBlackboard(this);
        public Blackboard originalBlackboard { get; private set; }
        public bool EditorDirty { get; set; }

        private List<BlackboardElement> _elementsList = new List<BlackboardElement>();

        public object context
        {
            get
            {
                if (scopeStack.TryPeek(out object e))
                    return e;
                return null;
            }
        }

        Stack<object> scopeStack = new Stack<object>();

        //only use through sandboxing
        private BlackboardManager bbmanager;

        [SerializeField, HideInInspector] bool isRuntime;

        public Mode mode => _serializedMode;
        [SerializeField, HideInInspector] Mode _serializedMode = Mode.OnlyEditor;

        public struct BBScope : IDisposable
        {
            public object Caller;
            public Blackboard blackboard;
            public BBScope(object caller, Blackboard bb)
            {
                this.Caller = caller;
                this.blackboard = bb;
                blackboard.scopeStack.Push(caller);
            }
            public void Dispose()
            {
                blackboard.scopeStack.Pop();
                Caller = null;
            }
        }

        public void SetupRuntime(Blackboard editorBlackboard, BlackboardManager manager)
        {
            this.bbmanager = manager;
            this.originalBlackboard = editorBlackboard;
            this.isRuntime = true;
            this.runtimeElements = new Dictionary<string, BlackboardElement>();
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (f.FieldType.IsSubclassOf(typeof(BlackboardElement)))
                {
                    BlackboardElement element = f.GetValue(this) as BlackboardElement;
                    element.SetupRuntime(this, f.Name);
                    runtimeElements.Add(f.Name, element);
                    _elementsList.Add(element);
                }
            }
        }

        public void ObserveEditorChanges()
        {
            for (int i = 0; i < _elementsList.Count; i++)
            {
                _elementsList[i].ObserveEditorChanges();
            }
        }

        public BBScope Scope(object caller)
        {
            return new BBScope(caller, this);
        }

        protected void GetBlackboard(string name)
        {
            bbmanager.GetBlackboard(name);
        }

        private void OnValidate()
        {
            EditorDirty = true;
        }
    }
}