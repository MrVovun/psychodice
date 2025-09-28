using BlackboardSource;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraState", menuName = "_Dice/Blackboard/CameraState", order = -1000)]
public class BB_CameraState : Blackboard
{
    public BlackboardValue<CameraState> cameraState = new BlackboardValue<CameraState>();
}
