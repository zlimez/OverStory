using System.Collections;
using UnityEngine;

namespace Environment.Enemy.Anim
{
    public class BloomAnim : PortraitAnim
    {
        [SerializeField] Transform target;

        public enum State
        {
            Idle,
            Death,
        }

        protected override void Awake()
        {
            base.Awake();
            currState = State.Idle.ToString();
        }

        void Start()
        {
            StartCoroutine(TestMoveTo(target.position));
        }

        IEnumerator TestMoveTo(Vector3 target)
        {
            yield return new WaitForSeconds(1f);
            portrait.StopAll();
            var startPos = portrait.GetBoneSocket("tongue").position;
            Debug.Log("Start pos: " + startPos);
            Debug.Log("Target pos: " + target);
            // interpolate between start and target
            var t = 0f;
            while (t < 1f)
            {
                MoveTo(Vector3.Lerp(startPos, target, t));
                t += Time.deltaTime;
                yield return null;
            }
            while (t < 2f)
            {
                MoveTo(Vector3.Lerp(target, startPos, t - 1));
                t += Time.deltaTime;
                yield return null;
            }
            MoveTo(startPos);
            yield return new WaitForSeconds(0.1f);
            portrait.CrossFade(State.Idle.ToString(), 0.5f);
        }

        protected override void OnEnable()
        {
            _playDeathAnim = () => TransitionToState(State.Death.ToString());
            base.OnEnable();
        }

        public void MoveTo(Vector3 position)
        {
            var tongue = portrait.GetBone("tongue");
            portrait.SetBoneIK(tongue, position, Space.World);
        }

        public void LookAt(float angle)
        {
            var tongue = portrait.GetBone("tongue");
            portrait.SetBoneRotation(tongue, angle, Space.World);
        }
    }
}