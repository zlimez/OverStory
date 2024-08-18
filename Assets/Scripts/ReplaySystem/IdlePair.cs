using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepBreath.ReplaySystem
{
    public class IdlePair : MonoBehaviour
    {
        public GameObject spiritualObject;
        public GameObject realObject;

        void OnEnable()
        {
            EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, TurnOnRealWorld);
            EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, TurnOnSpiritWorld);
        }

        void OnDisable()
        {
            EventManager.StopListening(StaticEvent.Core_SwitchToRealWorld, TurnOnRealWorld);
            EventManager.StopListening(StaticEvent.Core_SwitchToOtherWorld, TurnOnSpiritWorld);
        }

        private void TurnOnSpiritWorld(object input = null)
        {
            realObject.SetActive(false);
            spiritualObject.SetActive(true);
        }

        private void TurnOnRealWorld(object input = null)
        {
            realObject.SetActive(true);
            spiritualObject.SetActive(false);
        }
    }
}
