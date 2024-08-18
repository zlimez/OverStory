using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.EventSystem;

public class Grabber : MonoBehaviour
{
    private Grabbable GrabbedObject;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject releaseKey;

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.R) && IsGrabbing())
        {
            Release();
        }
    }
    
    public bool IsGrabbing()
    {
        return GrabbedObject != null;
    }

    public void Grab(Grabbable interactedObject) {
        if (GrabbedObject == null) {
            // Grab
            GrabbedObject = interactedObject;
            animator.SetBool("isHolding", true);
            EventManager.InvokeEvent(StaticEvent.Common_GrabStateChanged, true);
        }
        if (releaseKey != null)
            releaseKey.SetActive(true);
    }

    public void Release()
    {
        animator.SetBool("isHolding", false);
        GetComponent<SwimController>().IsRotationFrozen = false;
        EventManager.InvokeEvent(StaticEvent.Common_GrabStateChanged, false);
        GrabbedObject.Released();
        GrabbedObject = null;
        if (releaseKey != null)
            releaseKey.SetActive(false);
    }
}
