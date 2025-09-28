using System.Collections.Generic;
using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "_Dice/Blackboard/GameRules", order = -1000)]
public class BB_GameRules : Blackboard
{
    public BlackboardValue<int> maxDicesInPlayerHand = new BlackboardValue<int>();
    public BlackboardValue<int> matchesPerAnte = new BlackboardValue<int>();
}
