using System.Collections;
using System.Collections.Generic;
using Chronellium.EventSystem;
using UnityEngine;
using DigitalRuby.Tween;

public class EnterOtherWorldLogic : MonoBehaviour
{
    public AudioClip rainBGM;
    public AudioClip underWaterBGM;
    public AudioSource audioSource;
    public GameObject normalWorldInteractbles;
    public GameObject spiritWorldInteractbles;
    public SpriteRenderer origSprite;
    public Rigidbody playerSwimControllerRB;
    public enum Realm { otherWorld, realWorld };
    public Realm realm = Realm.realWorld;
    public float OtherWorldMomentEntered { get; private set; }
    public float TimeSinceOtherWorldEntered => Time.timeSinceLevelLoad - OtherWorldMomentEntered;
    private bool canTrigger = true;
    public void SwitchRealm(bool isForced = false)
    {
        IEnumerator ResetTriggerFlag(float delay)
        {
            yield return new WaitForSeconds(delay);
            canTrigger = true;
        }

        if (canTrigger)
        {
            if (realm == Realm.realWorld)
            {
                realm = Realm.otherWorld;
                OtherWorldMomentEntered = Time.timeSinceLevelLoad;
                EventManager.InvokeEvent(StaticEvent.Core_SwitchToOtherWorld);


                Vector3 dir = new Vector3(0, 310, 0);
                playerSwimControllerRB.AddForce(dir);
                System.Action<ITween<float>> BandInCallBack = (t) =>
               {

                   origSprite.color = new Color(t.CurrentValue, t.CurrentValue, t.CurrentValue, t.CurrentValue);
               };

                gameObject.Tween("FadeIn", 1.0f, 0.0f, 1.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
                audioSource.clip = underWaterBGM;
                audioSource.Play();

                BreathTimer.Instance.gameObject.SetActive(false);
            }
            else
            {
                realm = Realm.realWorld;
                EventManager.InvokeEvent(StaticEvent.Core_SwitchToRealWorld, isForced);
            }

            normalWorldInteractbles.SetActive(false);
            spiritWorldInteractbles.SetActive(true);
            canTrigger = false;
            StartCoroutine(ResetTriggerFlag(1f)); // Reset the flag after x seconds
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        SwitchRealm();
    }
}
