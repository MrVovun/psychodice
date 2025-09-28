using System;
using System.Collections;
using System.Collections.Generic;
using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "Debug", menuName = "_Dice/Blackboard/Debug", order = -1000)]
public class BB_Debug : Blackboard
{
    [Serializable]
    public class DebugDieLaunch
    {
        public BaseDie die;
        public int desiredOutcome;
    }

    public BlackboardEvent debugLaunchDiceRequest = new BlackboardEvent();
    public BlackboardValue<bool> useDesiredOutcome = new BlackboardValue<bool>();
    public BlackboardValue<List<DebugDieLaunch>> debugPopulatePlayerSelection = new BlackboardValue<List<DebugDieLaunch>>();
}
