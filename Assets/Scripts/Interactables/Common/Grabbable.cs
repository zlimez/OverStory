using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepBreath.Environment;

public class Grabbable : Interactable
{
    private bool isGrabbed = false;
    private Follower follower;


    // Start is called before the first frame update
    void Awake()
    {
        follower = GetComponent<Follower>();
    }

    public override void Interact()
    {
        if (Player.GetComponent<Grabber>().IsGrabbing()) return;
        Player.GetComponent<Grabber>().Grab(this);
        isGrabbed = true;
        follower.target = Player;
        follower.enabled = true;
        
        if (StateManager.realm == Realm.otherWorld) {
            Player.GetComponent<SwimController>().IsRotationFrozen = isGrabbed;
        }
    }

    public void Released()
    {
        isGrabbed = false;
        follower.enabled = false;
    }
}
