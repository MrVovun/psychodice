using UnityEngine;

public class GameLoopSystem : SystemBase<BB_Dummy>
{
    BB_GameState gameState;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
    }

    protected override void Start()
    {
        base.Start();
        // faking advancing game loop
        using(gameState.Scope(this))
        {
            gameState.gameLoopState.Value = GameLoopState.Menu;
            gameState.gameLoopState.Value = GameLoopState.Playing;
        }
    }
}
