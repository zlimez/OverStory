using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle1Puddle : Interactable
{
    public GameObject normCharacter;
    public GameObject swimCharacter;
    public GameObject normCharacterAppearPosition;
    public GameObject normCharacterEndingPosition;
    public GameObject swimCharacterAppearPosition;
    public GameObject swimCharacterEndingPosition;

    public float normLerpSpeed = 100;
    public float waterLerpSpeed = 200;

    private bool isInRealWorld = true;
    public override void Interact()
    {
        if (isInRealWorld)
        {
            EventManager.InvokeEvent(StaticEvent.Core_SwitchToOtherWorld);
            isInRealWorld = false;
            DropIntoWater();
        }
        else
        {
            EventManager.InvokeEvent(StaticEvent.Core_SwitchToRealWorld);
            isInRealWorld = true;
            JumpOutOfWater();
        }
    }

    private void DropIntoWater()
    {
        swimCharacter.transform.position = swimCharacterAppearPosition.transform.position;
        swimCharacter.SetActive(true);
        normCharacter.SetActive(false);
        // TODO: Disabled until fixed
        //StartCoroutine(DropIntoWaterMovement());
    }

    private void JumpOutOfWater()
    {
        normCharacter.transform.position = normCharacterAppearPosition.transform.position;
        normCharacter.SetActive(true);
        swimCharacter.SetActive(false);
        // TODO: Disabled until fixed
        //StartCoroutine(JumpOutOfWaterMovement());
    }

    public IEnumerator DropIntoWaterMovement()
    {
        while (swimCharacter.transform.position != swimCharacterEndingPosition.transform.position)
        {
            //Debug.Log("Moving in water");
            swimCharacter.transform.position = Vector3.MoveTowards(swimCharacter.transform.position, swimCharacterEndingPosition.transform.position, 2f);
            yield return null;
        }
    }

    public IEnumerator JumpOutOfWaterMovement()
    {
        while (normCharacter.transform.position != normCharacterEndingPosition.transform.position)
        {
            //Debug.Log("Moving in air");
            normCharacter.transform.position = Vector3.MoveTowards(normCharacter.transform.position, normCharacterEndingPosition.transform.position, 0.05f);
            yield return null;
        }
    }
}
