using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraSwitcher : MonoBehaviour
{
    // Start is called before the first frame update

    public static CameraSwitcher instance = null;
    [SerializeField]
    private CinemachineVirtualCamera[] cameras;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            if (camera == newCamera)
            {
                camera.Priority = 10;
            }
            else
            {
                camera.Priority = 0;
            }
        }
    }


}
