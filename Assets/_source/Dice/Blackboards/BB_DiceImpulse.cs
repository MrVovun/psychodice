using System;
using System.Collections;
using System.Collections.Generic;
using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceImpulse", menuName = "_Dice/Blackboard/DiceImpulse", order = -1000)]
public class BB_DiceImpulse : Blackboard
{
    public float force;
    public float forceRandomDeviation;
    public float angularForce;
    public float angularForceRandomDeviation;
    public ForceMode forceMode;
    public bool randomize;
    public float randomDeviation;
}
