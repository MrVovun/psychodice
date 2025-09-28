using BlackboardSource;
using UnityEngine;

public abstract class SystemBase : MonoBehaviour
{
    protected BlackboardManager blackboardManager;

    public abstract void SystemSetup(BlackboardManager blackboardManager);

    protected Blackboard GetBlackboard(string name)
    {
        return blackboardManager.GetBlackboard(name);
    }

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
}

public abstract class SystemBase<T> : SystemBase where T : Blackboard
{
    [SerializeField] private T _blackboard;
    protected T blackboard => _blackboard;

    public override void SystemSetup(BlackboardManager blackboardManager)
    {
        this.blackboardManager = blackboardManager;
        if (_blackboard)
        {
            // replace linked editor BB with runtime version
            _blackboard = blackboardManager.EditorToRuntime(_blackboard) as T;
        }
    }
}
