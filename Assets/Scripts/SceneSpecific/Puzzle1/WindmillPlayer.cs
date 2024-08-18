using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Chronellium.EventSystem;

public class WindmillPlayer : Interactable
{
    [SerializeField] private Transform[] engagePoints;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private float zoomOutDistance;
    private Transform playerCameraPoint;
    [SerializeField] private float swimmerSpeed = 2.5f;
    private Transform engagedPoint;
    private SwimController swimmer;
    private bool isEngaged;
    [SerializeField] private WindmillRotator rotator;
    private IEnumerator swimToRoutine;
    private IEnumerator swimWithRoutine;
    private Vector3 originalScale;

    public override void Interact()
    {
        if (isEngaged) {
            isEngaged = false;
            swimmer.Automated = false;
            swimmer.SwimmerAnimator.SetBool("isHolding", false);
            swimmer.SwimmerRenderer.transform.localScale = originalScale;
            if (swimToRoutine != null) {
                StopCoroutine(swimToRoutine);
                rotator.StartPowerDown();
            }

            if (swimWithRoutine != null) {
                StopCoroutine(swimWithRoutine);
                rotator.StartPowerDown();
            }

            mainCamera.SetFollowTransform(playerCameraPoint, mainCamera.DefaultDistance);
            EventManager.InvokeEvent(DynamicEvent.DisengagedWindmill);
        } else {
            isEngaged = true;
            swimmer = Player.GetComponent<SwimController>();
            originalScale = swimmer.SwimmerRenderer.transform.localScale;
            swimmer.Automated = true;
            swimToRoutine = MovePlayerToBlade();
            StartCoroutine(swimToRoutine);

            playerCameraPoint = mainCamera.FollowTransform;
            mainCamera.SetFollowTransform(centerPoint, zoomOutDistance);
            EventManager.InvokeEvent(DynamicEvent.EngagedWindmill);
        }
    }

    // NOTE: Sprite flip point at 0 and 180 degrees -> + / - y relative to center of windmill blades
    IEnumerator MovePlayerWithBlade() {
        float timeSinceLastFlip = 0;
        while (true) {
            bool isRight = rotator.IsRotatingClockwise ? swimmer.transform.position.y - centerPoint.position.y < 0 : swimmer.transform.position.y - centerPoint.position.y > 0;
            if (timeSinceLastFlip >= 0.1) {
                  if (isRight) {
                    swimmer.SwimmerRenderer.transform.localScale = originalScale;
                } else {
                    swimmer.SwimmerRenderer.transform.localScale = new Vector3(-1 * originalScale.x, originalScale.y, originalScale.z);
                }
                timeSinceLastFlip = 0;
            }
            timeSinceLastFlip += Time.deltaTime;
            Vector2 toGrabPoint = swimmer.Grabpoint.position - swimmer.transform.position;

            Vector2 positionAftOffset = (Vector2)engagedPoint.position - toGrabPoint;
            swimmer.transform.position = new Vector3(positionAftOffset.x, positionAftOffset.y, swimmer.transform.position.z);
            yield return null;
        }
    }

    IEnumerator MovePlayerToBlade() {
        engagedPoint = engagePoints.Aggregate(engagePoints[0], (pt1, pt2) => {
            if ((pt2.position - swimmer.transform.position).sqrMagnitude < (pt1.position - swimmer.transform.position).sqrMagnitude) {
                return pt2;
            }
                return pt1;
            }
        );

        Vector2 toEngagePt = engagedPoint.position - swimmer.Grabpoint.position;
        float timeNeeded = toEngagePt.magnitude / swimmerSpeed;
        float timeElapsed = 0;
        bool isToRight = toEngagePt.x > 0;
        if (isToRight) {
            swimmer.SwimmerRenderer.transform.localScale = originalScale;
        } else {
            swimmer.SwimmerRenderer.transform.localScale = new Vector3(-1 * originalScale.x, originalScale.y, originalScale.z);
        }
        // To account for possible flip
        toEngagePt = engagedPoint.position - swimmer.Grabpoint.position;
        Vector2 targetPt = (Vector2)swimmer.transform.position + toEngagePt;
        Vector3 initPosition = swimmer.transform.position;

        while (timeElapsed < timeNeeded) {
            swimmer.transform.position = VectorUtils.CubicLerpVector(initPosition, new Vector3(targetPt.x, targetPt.y, initPosition.z), timeElapsed / timeNeeded);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        swimmer.SwimmerAnimator.SetBool("isHolding", true);
        swimToRoutine = null;
        rotator.PowerOn();

        swimWithRoutine = MovePlayerWithBlade();
        StartCoroutine(swimWithRoutine);
    }
}
