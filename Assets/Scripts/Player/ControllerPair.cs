using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.EventSystem;
using DeepBreath.Environment;

public class ControllerPair : MonoBehaviour
{
    [SerializeField] private JumpAddedController normalController;
    [SerializeField] private SwimController swimController;
    [SerializeField] private GhostFader ghost;
    private Vector3 swimCharacterStartPosition;

    void OnEnable()
    {
        EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, ChangeToOther);
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, ChangeToReal);
    }

    void OnDisable()
    {
        EventManager.StopListening(StaticEvent.Core_SwitchToOtherWorld, ChangeToOther);
        EventManager.StopListening(StaticEvent.Core_SwitchToRealWorld, ChangeToReal);
    }

    private void ChangeToReal(object input = null)
    {
        bool isForced = (bool)input;
        if (isForced)
        {
            //StateManager.SwitchRealm();
            normalController.transform.position = Puddle.LastUsedPuddle.ForceSpawnPosition.position;
        }
        else
        {
            normalController.transform.position = swimController.transform.position;
            ghost.transform.position = swimCharacterStartPosition;
        }

        normalController.gameObject.SetActive(true);
        swimController.gameObject.SetActive(false);
    }

    private void ChangeToOther(object input = null)
    {
        swimController.transform.position = normalController.transform.position;
        swimCharacterStartPosition = swimController.transform.position;
        normalController.gameObject.SetActive(false);
        swimController.gameObject.SetActive(true);
    }

    private void ForceChangeToReal(object input = null)
    {

        normalController.gameObject.SetActive(true);
        swimController.gameObject.SetActive(false);
    }
}
