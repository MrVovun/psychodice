using System.Collections.Generic;
using BlackboardSource;
using UnityEngine;

public class Systems : MonoBehaviour
{

    public BlackboardManager blackboardManager => _blackboardManager;
    [SerializeField] private BlackboardManager _blackboardManager;

    List<SystemBase> _systems = new List<SystemBase>();

    private void Awake()
    {
        //Debug.Log("Preparing Blackboards");

        _blackboardManager.Initialize();

        //Debug.Log("Starting systems");

        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            var s = c.GetComponent<SystemBase>();
            if (s)
                _systems.Add(s);
        }

        for (int i = 0; i < _systems.Count; i++)
        {
            var s = _systems[i];
            //Debug.Log("Activating: " + s.name);

            s.enabled = false;
            s.SystemSetup(_blackboardManager);
            s.enabled = true;
        }
    }

    private void LateUpdate()
    {
        _blackboardManager.ManualUpdate();
    }
}
