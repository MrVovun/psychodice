using UnityEngine;

public class UIDiceRoll : MonoBehaviour
{
    [SerializeField] BB_UIState uiState;

    private void Awake()
    {
        uiState = uiState.runtimeCopy as BB_UIState;
        uiState.playingUIState.AddListenerAndUpdate(OnPlayingUIStateChanged);
        gameObject.SetActive(false);
    }

    private void OnPlayingUIStateChanged(PlayingUIState state)
    {
        switch (state)
        {
            case PlayingUIState.DiceRoll:
                gameObject.SetActive(true);
                break;
            case PlayingUIState.Declaration:
            case PlayingUIState.Pause:
            default:
                gameObject.SetActive(false);
                break;
        }
    }
}
