using System;
using BlackboardSource;
using UnityEngine;

public abstract class DiceVirtualCameraBase : MonoBehaviour
{
    [SerializeField] CameraState bindToCameraState;
    [SerializeField] BB_CameraState cameraState;

    protected virtual void Awake()
    {
        cameraState = cameraState.runtimeCopy as BB_CameraState;
        cameraState.cameraState.AddListenerAndUpdate(OnCameraStateChanged);
    }

    private void OnCameraStateChanged(CameraState state)
    {
        gameObject.SetActive(state == bindToCameraState);
    }
}
