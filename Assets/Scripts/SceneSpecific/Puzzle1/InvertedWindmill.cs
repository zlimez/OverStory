using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedWindmill : Interactable
{
    public GameObject windmillBlades;
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private float zoomOutDistance;
    [SerializeField] private Transform cameraFollowPoint;
    private Transform playerFollowPoint;
    public string choiceText1, choiceText2;
    private Choice choice1, choice2;
    private bool choice1Activated = true;
    private bool choice2Activated = true;
    private IEnumerator turnClockwise;
    private IEnumerator turnAntiClockwise;
    private float rotationZ;


    void Awake()
    {
        InitialiseChoice();
        turnClockwise = TurnClockwise();
        turnAntiClockwise = TurnAntiClockwise();
        rotationZ = windmillBlades.transform.rotation.z;
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, ResetBlades);
    }

    private void InitialiseChoice()
    {
        choice1 = new Choice(choiceText1, Choice1, choice1Activated);
        choice2 = new Choice(choiceText2, Choice2, choice2Activated);
    }

    public override void Interact()
    {
        ChoiceManager.Instance.StartChoice(choice1, choice2);
    }

    // Invoked Events
    // Change according to what is required
    public void Choice1(object o = null)
    {
        // Turn Clockwise
        StopCoroutine(turnClockwise);
        StopCoroutine(turnAntiClockwise);
        StartCoroutine(turnClockwise);
        EventManager.InvokeEvent(DynamicEvent.EngagedWindmill);
        playerFollowPoint = mainCamera.FollowTransform;
        mainCamera.SetFollowTransform(cameraFollowPoint, zoomOutDistance);
    }

    public void Choice2(object o = null)
    {
        // Turn Anti clockwise
        StopCoroutine(turnClockwise);
        StopCoroutine(turnAntiClockwise);
        StartCoroutine(turnAntiClockwise);
        EventManager.InvokeEvent(DynamicEvent.EngagedWindmill);
        playerFollowPoint = mainCamera.FollowTransform;
        mainCamera.SetFollowTransform(cameraFollowPoint, zoomOutDistance);
    }

    public IEnumerator TurnClockwise()
    {
        while (true)
        {
            rotationZ += 1f;
            windmillBlades.transform.eulerAngles = new Vector3(windmillBlades.transform.eulerAngles.x, windmillBlades.transform.eulerAngles.y, rotationZ);
            yield return null;
        }
    }

    public IEnumerator TurnAntiClockwise()
    {
        while (true)
        {
            rotationZ -= 1f;
            windmillBlades.transform.eulerAngles = new Vector3(windmillBlades.transform.eulerAngles.x, windmillBlades.transform.eulerAngles.y, rotationZ);
            yield return null;
        }
    }

    protected override void OnTriggerExit(Collider collision)
    {
        base.OnTriggerExit(collision);
        mainCamera.SetFollowTransform(playerFollowPoint, mainCamera.DefaultDistance);
    }

    private void ResetBlades(object input = null)
    {
        StopCoroutine(turnClockwise);
        StopCoroutine(turnAntiClockwise);
        windmillBlades.transform.rotation = Quaternion.Euler(windmillBlades.transform.rotation.x, windmillBlades.transform.rotation.y, 0);
    }
}
