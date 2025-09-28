namespace BlackboardSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class BlackboardManager : MonoBehaviour
    {
        [SerializeField] List<Blackboard> runtimeBlackboards = new List<Blackboard>();
        [SerializeField] List<Blackboard> editorBlackboards = new List<Blackboard>();

        public static BlackboardManager Instance { get; private set; }
        public static bool IsPlaying { get; private set; }

        private bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            IsPlaying = true;
        }

        public void Initialize()
        {
            if (initialized)
                return;
            Instance = this;
            initialized = true;

            runtimeBlackboards.Clear();
            // create runtime copies of blackboards
            List<Blackboard> runtime = new List<Blackboard>();
            foreach (var bb in editorBlackboards)
            {
                EditorToRuntime(bb);
            }
        }

        public void ManualUpdate()
        {
            for (int i = 0; i < runtimeBlackboards.Count; i++)
                runtimeBlackboards[i].ObserveEditorChanges();
        }

        public Blackboard EditorToRuntime(Blackboard editorBb)
        {
            if (editorBb.IsRuntime)
            {
                Debug.LogError("Blackboard is already Runtime.");
                return null;
            }

            for (int i = 0; i < runtimeBlackboards.Count; i++)
            {
                var bb = runtimeBlackboards[i];
                if (bb.originalBlackboard == editorBb)
                    return bb;
            }

            Blackboard newBb = GameObject.Instantiate(editorBb);
            newBb.name = editorBb.name;
            newBb.SetupRuntime(editorBb, this);
            runtimeBlackboards.Add(newBb);
            return newBb;
        }

        public Blackboard GetBlackboard(string name)
        {
            return runtimeBlackboards.FirstOrDefault(x => x.name == name);
        }

        public Blackboard GetBlackboard(Blackboard editorBlackboard)
        {
            if (!editorBlackboard)
                return null;

            if(editorBlackboard.IsRuntime) // already runtime
                return editorBlackboard;

            return runtimeBlackboards.FirstOrDefault(x => x.originalBlackboard == editorBlackboard);
        }
    }
}