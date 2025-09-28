using System;
using UnityEngine;

public class CameraSystem : SystemBase<BB_Dummy>
{
    BB_GameState gameState;
    BB_CameraState cameraState;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
        cameraState = GetBlackboard(DiceBlackboards.CameraState) as BB_CameraState;

        gameState.matchState.AddListener(OnMatchStateChanged);
    }

    private void OnMatchStateChanged(MatchState state)
    {
        using (cameraState.Scope(this))
        {
            switch (state)
            {
                case MatchState.MatchStarted:
                case MatchState.RoundStart:
                case MatchState.RoundDeclaration:
                    cameraState.cameraState.Value = CameraState.LockedOnTable;
                    break;
                case MatchState.RoundDiceRoll:
                    cameraState.cameraState.Value = CameraState.LockedOnDiceRoll;
                    break;
                case MatchState.RoundEnd:
                case MatchState.MatchEnded:
                    cameraState.cameraState.Value = CameraState.LockedOnTable;
                    break;
                default:
                    break;
            }
        }
    }
}
