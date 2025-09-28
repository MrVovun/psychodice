using System.Collections.Generic;
using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "_Dice/Blackboard/GameState", order = -1000)]
public class BB_GameState : BlackboardSource.Blackboard
{
    public BlackboardValue<GameLoopState> gameLoopState = new BlackboardValue<GameLoopState>();
    public BlackboardValue<RunState> runState = new BlackboardValue<RunState>();
    public BlackboardValue<Result> lastRunResult = new BlackboardValue<Result>();
    public BlackboardValue<AnteState> anteState = new BlackboardValue<AnteState>();
    public BlackboardValue<Result> lastAnteResult = new BlackboardValue<Result>();
    public BlackboardValue<int> currentAnte = new BlackboardValue<int>();

    public BlackboardValue<MatchState> matchState = new BlackboardValue<MatchState>();
    public BlackboardValue<Result> lastMatchResult = new BlackboardValue<Result>();
    public BlackboardValue<int> currentMatch = new BlackboardValue<int>();
    public BlackboardValue<int> currentRound = new BlackboardValue<int>();

    public BlackboardEvent playerFinishedDeclaring = new BlackboardEvent();
    public BlackboardEvent diceRollComplete = new BlackboardEvent();

    public BlackboardValue<List<BaseDie>> playerSelectedDice = new BlackboardValue<List<BaseDie>>(); 
    public BlackboardValue<List<BaseDie>> opponentSelectedDice = new BlackboardValue<List<BaseDie>>();
}
