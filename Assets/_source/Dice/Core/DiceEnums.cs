public enum GameLoopState
{
    Menu,
    Playing,
    Paused
}

public enum RunState
{
    WaitingToStartRun,
    NewRunStarted,
    RunInProgress,
    RunEnded
}

public enum AnteState
{
    NewAnteStarted,
    IntroductionStarted,
    Preparation,
    StartingNewMatch,
    AnteEnded
}

public enum MatchState
{
    MatchStarted,
    RoundStart,
    RoundDeclaration,
    RoundDiceRoll,
    RoundEnd,
    MatchEnded,
}

public enum Result
{
    Win,
    Loss
}

public enum GlobalUIState
{
    MainMenu,
    Playing,
}

public enum MainMenuUIState
{
    MainMenu,
    Settings,
    Credits
}

public enum PlayingUIState
{
    Declaration,
    DiceRoll,
    Pause,
}

public enum CameraState
{
    ControlledByCutscene,
    FreeViewTable,
    LockedOnTable,
    LockedOnJester,
    LockedOnDiceRoll,
}

public enum DieSides
{
    D4 = 4,
    D6 = 6,
    D20 = 20
}