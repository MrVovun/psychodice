using System;
using UnityEngine;

public class UISystem : SystemBase<BB_Dummy>
{
    private BB_GameState gameState;
    private BB_UIState uiState;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
        uiState = GetBlackboard(DiceBlackboards.UIState) as BB_UIState;

        gameState.gameLoopState.AddListener(OnGameLoopStateChanged);
        gameState.matchState.AddListener(OnMatchStateChanged);
    }

    private void OnMatchStateChanged(MatchState state)
    {
        using (uiState.Scope(this))
        {
            switch (state)
            {
                case MatchState.MatchStarted:
                case MatchState.RoundStart:
                case MatchState.RoundDeclaration:
                    uiState.playingUIState.Value = PlayingUIState.Declaration;
                    break;
                case MatchState.RoundDiceRoll:
                    uiState.playingUIState.Value = PlayingUIState.DiceRoll;
                    break;
                case MatchState.RoundEnd:
                    break;
                case MatchState.MatchEnded:
                    break;
                default:
                    break;
            }
        }
    }

    private void OnGameLoopStateChanged(GameLoopState state)
    {
        using (uiState.Scope(this))
        {
            switch (state)
            {
                case GameLoopState.Menu:
                    uiState.globalUiState.Value = GlobalUIState.MainMenu;
                    break;
                case GameLoopState.Playing:
                    uiState.globalUiState.Value = GlobalUIState.Playing;
                    break;
                case GameLoopState.Paused:
                    break;
                default:
                    break;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }
}
