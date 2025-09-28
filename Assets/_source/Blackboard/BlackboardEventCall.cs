using BlackboardSource;
using UnityEngine;

public class BlackboardEventCall : MonoBehaviour
{
    [SerializeField] string eventName;
    [SerializeField] Blackboard blackboard;

    Blackboard runtimeBb;

    private void Awake()
    {
        runtimeBb = BlackboardManager.Instance.GetBlackboard(blackboard);
    }

    public void Raise()
    {
        if(string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogError("Invalid event name.");
            return;
        }    

        if(runtimeBb && runtimeBb.IsRuntime && runtimeBb.runtimeElements.TryGetValue(eventName, out BlackboardElement elem))
        {
            if (elem is BlackboardEventBase ev)
            {
                ev.InvokeBoxed(this, null);
            }
        }
    }
}
