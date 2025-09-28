using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "UIState", menuName = "_Dice/Blackboard/UIState", order = -1000)]
public class BB_UIState : Blackboard
{
    public BlackboardValue<GlobalUIState> globalUiState = new BlackboardValue<GlobalUIState>();
    public BlackboardValue<MainMenuUIState> mainMenuUIState = new BlackboardValue<MainMenuUIState>();
    public BlackboardValue<PlayingUIState> playingUIState = new BlackboardValue<PlayingUIState>();
}
