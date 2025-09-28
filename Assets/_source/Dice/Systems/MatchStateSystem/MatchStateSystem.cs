using UnityEngine;

public class MatchStateSystem : SystemBase<BB_Dummy>
{
    private BB_GameState gameState;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;

        gameState.anteState.AddListener(OnAnteStateChanged);
        gameState.playerFinishedDeclaring.AddListener(OnPlayerFinishedDeclaring);
        gameState.diceRollComplete.AddListener(OnDiceRollComplete);
    }

    private void OnPlayerFinishedDeclaring()
    {
        using (gameState.Scope(this))
        {
            gameState.matchState.Value = MatchState.RoundDiceRoll;
            // hand off to dice roll handler
        }
    }

    private void OnDiceRollComplete()
    {
        using (gameState.Scope(this))
        {
            // determine round results
        }
    }

    private void OnAnteStateChanged(AnteState state)
    {
        switch (state)
        {
            case AnteState.NewAnteStarted:
                break;
            case AnteState.IntroductionStarted:
                break;
            case AnteState.Preparation:
                break;
            case AnteState.StartingNewMatch:
                using (gameState.Scope(this))
                {
                    gameState.matchState.Value = MatchState.MatchStarted;
                    gameState.currentRound.Value = 1;
                    gameState.matchState.Value = MatchState.RoundStart;
                    gameState.matchState.Value = MatchState.RoundDeclaration;
                }
                break;
            default:
                break;
        }
    }
}
