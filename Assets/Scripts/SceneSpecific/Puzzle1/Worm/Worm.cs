using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour
{
    public GameObject hitBox;
    public AudioSource audio;
    private bool isAttacking;

    void Start()
    {
        EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, ResetAttack);
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetAttack);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Player has entered worm bite range");
        if (isAttacking)
        {
            isAttacking = false;
            EventManager.InvokeEvent(StaticEvent.Core_ResetPuzzle);
        }
    }

    public void PlayChomp()
    {
        audio.Play();
    }
    // Called by the animation as an animation event
    public void Attack()
    {
        hitBox.SetActive(true);
    }

    public void ResetAttack(object input = null)
    {
        hitBox.SetActive(false);
    }

    private bool IsCableSeats(GameObject otherObject)
    {
        return otherObject.CompareTag("CableSeats");
    }

    private IEnumerator WaitBeforeReset(object input = null)
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.InvokeEvent(StaticEvent.Core_ResetPuzzle);
    }
}
