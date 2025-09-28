using UnityEngine;
using UnityEngine.UI;

public class UIDeclaration : MonoBehaviour
{
    [SerializeField] BB_GameState gameState;
    [SerializeField] BB_UIState uiState;
    [SerializeField] Button btn_declare;

    private void Awake()
    {
        uiState = uiState.runtimeCopy as BB_UIState;
        gameState = gameState.runtimeCopy as BB_GameState;

        uiState.playingUIState.AddListenerAndUpdate(OnPlayingUIStateChanged);
        OnPlayingUIStateChanged(uiState.playingUIState.Value);
        btn_declare.onClick.AddListener(ClickDeclare);
    }

    private void ClickDeclare()
    {
        gameState.playerFinishedDeclaring.Invoke(this);
    }

    private void OnPlayingUIStateChanged(PlayingUIState state)
    {
        switch (state)
        {
            case PlayingUIState.Declaration:
                gameObject.SetActive(true);
                break;
            case PlayingUIState.DiceRoll:
            case PlayingUIState.Pause:
            default:
                gameObject.SetActive(false);
                break;
        }
    }
}
