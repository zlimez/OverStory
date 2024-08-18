using Chronellium.EventSystem;
using DeepBreath.Environment;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialShadowRoom : MonoBehaviour
{
    public AIPath aiPath;
    // public GameObject shadowSprite;
    public GameObject shadowInitialPosition;
    private bool isIdle = true;

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            aiPath.canMove = false;
            return;
        }
        else
        {
            aiPath.canMove = true;
        }

        // if (aiPath.desiredVelocity.x >= 0.01f)
        // {
        //     transform.localScale = new Vector3(1f, 1f, 1f);
        // }
        // else if (aiPath.desiredVelocity.x <= -0.01f)
        // {
        //     transform.localScale = new Vector3(-1f, 1f, 1f);
        // }
    }

    private void Start()
    {
        // shadowSprite.SetActive(false);
        isIdle = true;
        aiPath = GetComponent<AIPath>();
        EventManager.StartListening(StaticEvent.Core_LowBreath, ShadowsAppear);
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, HideShadow);
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetShadow);
    }

    private void ShadowsAppear(object input = null)
    {
        Debug.Log("Shadow appears");
        // shadowSprite.SetActive(true);
        isIdle = false;
    }

    private void HideShadow(object input = null)
    {
        // shadowSprite.SetActive(false);
        isIdle = true;
    }

    private void ResetShadow(object input = null)
    {
        transform.position = shadowInitialPosition.transform.position;
        // shadowSprite.SetActive(false);
        isIdle = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            Debug.Log("Shadow touches player");
            EventManager.InvokeEvent(StaticEvent.Core_ResetPuzzle);
        }

    }
    private bool IsPlayer(GameObject otherObject)
    {
        return otherObject.CompareTag("Player");
    }
}
