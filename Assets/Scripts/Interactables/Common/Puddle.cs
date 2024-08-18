using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.EventSystem;

namespace DeepBreath.Environment
{
    public enum Realm { otherWorld, realWorld }
    public static class StateManager
    {
        public static Realm realm = Realm.realWorld;
        public static float OtherWorldMomentEntered { get; private set; }
        public static float TimeSinceOtherWorldEntered => Time.timeSinceLevelLoad - OtherWorldMomentEntered;
        public static void SwitchRealm(bool isForced = false)
        {
            Debug.Log("Realm is " + realm);
            if (realm == Realm.realWorld)
            {
                realm = Realm.otherWorld;
                OtherWorldMomentEntered = Time.timeSinceLevelLoad;
                EventManager.InvokeEvent(StaticEvent.Core_SwitchToOtherWorld);
            }
            else
            {
                realm = Realm.realWorld;
                EventManager.InvokeEvent(StaticEvent.Core_SwitchToRealWorld, isForced);
            }
        }
    }

    public class Puddle : Interactable
    {
        public static Puddle LastUsedPuddle { get; private set; }
        public Transform ForceSpawnPosition;
        public AudioSource SubmergeSound;
        public AudioSource ExitWaterSound;

        public override void Interact()
        {
            if (StateManager.realm == Realm.realWorld)
            {
                SubmergeSound.Play();
            }
            else
            {
                ExitWaterSound.Play();
            }
            LastUsedPuddle = this;
            StateManager.SwitchRealm();
        }
    }
}
