namespace BlackboardSource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class BlackboardElement
	{
        public string name { get; private set; }
        public Blackboard blackboard { get; private set; }
        public bool Runtime { get; private set; }

        public void SetupRuntime(Blackboard blackboard, string valueName)
        {
            this.name = valueName;
            this.blackboard = blackboard;
            this.Runtime = true;
        }

        public virtual void ObserveEditorChanges()
        {
        }
    }

}