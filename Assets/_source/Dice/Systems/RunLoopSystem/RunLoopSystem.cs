using UnityEngine;

public class RunLoopSystem : SystemBase<BB_Dummy>
{
    BB_GameState gameState;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
        gameState.gameLoopState.AddListener(OnGameLoopStateChanged);
        gameState.anteState.AddListener(OnAnteStateChanged);
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
                break;
            case AnteState.AnteEnded:
                using (gameState.Scope(this))
                {
                    if (gameState.lastAnteResult.Value == Result.Loss)
                    {
                        gameState.lastRunResult.Value = Result.Loss;
                        gameState.runState.Value = RunState.RunEnded;
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnGameLoopStateChanged(GameLoopState state)
    {
        switch (state)
        {
            case GameLoopState.Menu:
                break;
            case GameLoopState.Playing:
                if (gameState.runState.Value == RunState.WaitingToStartRun)
                {
                    using (gameState.Scope(this))
                    {
                        gameState.runState.Value = RunState.WaitingToStartRun;
                        gameState.runState.Value = RunState.NewRunStarted;
                        gameState.runState.Value = RunState.RunInProgress;
                    }
                }
                break;
            case GameLoopState.Paused:
                break;
            default:
                break;
        }
    }
}
