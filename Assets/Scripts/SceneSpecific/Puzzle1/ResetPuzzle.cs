using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepBreath.ReplaySystem;
using Chronellium.EventSystem;
using Tuples;

public class ResetPuzzle : MonoBehaviour
{
    public GameObject normPlayer;
    public GameObject swimPlayer;
    public GameObject cableSeatsInitialPosition;
    public GameObject normPlayerInitialPosition;
    public GameObject swimPlayerInitialPosition;
    public ActionReplayPair<TransformRecorder, TransformReplayer, Pair<Vector3, Quaternion>> windmillActionReplayPair;
    public ActionReplayPair<TransformRecorder, TransformReplayer, Pair<Vector3, Quaternion>> cableSeatsActionReplayPair;
    public CableSeatsOW cableSeatsOW;
    public CableSeats cableSeats;
    public GameObject windmillBlades;
    public WindmillRotator millRotator;
    public Worm worm;

    private void Start()
    {
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetPuzzle1);
    }

    public void ResetPuzzle1(object input = null)
    {
        //EventManager.InvokeEvent(StaticEvent.Core_SwitchToRealWorld, true); //bool isForced = true
        //StopAllCoroutines();
        windmillActionReplayPair.ResetReplayRecords();
        windmillBlades.transform.eulerAngles = Vector3.zero;
        millRotator.Reset();
        cableSeatsActionReplayPair.ResetReplayRecords();
        cableSeatsOW.StopAllCoroutines();
        cableSeatsOW.isMoving = false;
        cableSeatsOW.transform.position = cableSeatsInitialPosition.transform.position;
        cableSeatsOW.StopAllCoroutines();
        // Teleport player back to start
        cableSeats.isSeated = false;
        cableSeats.transform.position = cableSeatsInitialPosition.transform.position;
        normPlayer.transform.position = normPlayerInitialPosition.transform.position;
        swimPlayer.transform.position = swimPlayerInitialPosition.transform.position;
        Rigidbody rb = normPlayer.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        worm.ResetAttack();
    }
}
