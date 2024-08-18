using UnityEngine;
using Cinemachine;

public class RoomCutsceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera newCamera;
    public CinemachineVirtualCamera exitCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (newCamera)
        {
            CameraSwitcher.instance.SwitchCamera(newCamera);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (exitCamera)
        {
            CameraSwitcher.instance.SwitchCamera(exitCamera);
        }
    }
}
