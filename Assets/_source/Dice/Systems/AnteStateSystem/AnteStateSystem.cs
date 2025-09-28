using System;
using UnityEngine;

public class AnteStateSystem : SystemBase<BB_Dummy>
{
    BB_GameState gameState;
    BB_GameRules gameRules;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
        gameRules = GetBlackboard(DiceBlackboards.GameRules) as BB_GameRules;

        gameState.runState.AddListener(OnRunStateChanged);
        gameState.matchState.AddListener(OnMatchStateChanged);
    }

    private void OnRunStateChanged(RunState state)
    {
        switch (state) 
        {
            case RunState.WaitingToStartRun:  
                break;
            case RunState.NewRunStarted:
                using (gameState.Scope(this))
                {
                    gameState.currentAnte.Value = 1;
                    gameState.anteState.Value = AnteState.NewAnteStarted;
                    gameState.anteState.Value = AnteState.Preparation;
                    gameState.anteState.Value = AnteState.StartingNewMatch; 
                }
                break;
            case RunState.RunInProgress:
                break;
            case RunState.RunEnded:
                break;
            default:
                break;
        }
    }

    private void OnMatchStateChanged(MatchState state)
    {
        switch (state)
        {
            case MatchState.MatchEnded:
                // process match results and make decisions
                using(gameState.Scope(this))
                {
                    if(gameState.lastMatchResult.Value == Result.Win)
                    {
                        int nextMatchNumber = gameState.currentMatch.Value + 1;
                        if (nextMatchNumber > gameRules.matchesPerAnte.Value)
                        {
                            // ante complete
                            gameState.currentAnte.Value += 1;
                            gameState.currentMatch.Value = 1;
                            gameState.anteState.Value = AnteState.Preparation;
                        }
                        else
                        {
                            // continue to next match
                            gameState.currentMatch.Value += 1;
                            gameState.anteState.Value = AnteState.StartingNewMatch;
                        }
                    }
                    else
                    {
                        gameState.anteState.Value = AnteState.AnteEnded;
                    }
                }
                break;
            default:
                break;
        }
    }
}
