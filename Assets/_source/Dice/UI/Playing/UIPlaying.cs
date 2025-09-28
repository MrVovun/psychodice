using System;
using UnityEngine;

public class UIPlaying : MonoBehaviour
{
    [SerializeField] BB_UIState uiState;
    [SerializeField] BB_GameState gameState;

    private void Awake()
    {
        gameState = gameState.runtimeCopy as BB_GameState;
        uiState = uiState.runtimeCopy as BB_UIState;

        uiState.globalUiState.AddListenerAndUpdate(OnGlobalUIStateChanged);
        gameObject.SetActive(false);
    }

    private void OnGlobalUIStateChanged(GlobalUIState state)
    {
        switch (state)
        {
            case GlobalUIState.MainMenu:
                gameObject.SetActive(false);
                break;
            case GlobalUIState.Playing:
                gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
