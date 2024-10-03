using UnityEngine;

namespace Abyss.Environment.Enemy.Anim
{
    public class BloomAnim : PortraitAnim
    {
        // [SerializeField] Transform target;
        // [SerializeField] AnimationCurve curve;
        // [SerializeField] float chompTime;
        AnyPortrait.apOptBone tongue;

        public enum State
        {
            Idle,
            Death,
            Wiggle,
            MouthOpen,
            MouthClose
        }

        protected override void Awake()
        {
            base.Awake();
            currState = State.Idle.ToString();
        }

        // public void Chomp()
        // {
        //     StartCoroutine(TestMoveTo(target.position));
        // }

        // IEnumerator TestMoveTo(Vector3 target)
        // {
        //     TransitionToState(State.MouthOpen.ToString());
        //     yield return new WaitForSeconds(0.2f);
        //     var startPos = portrait.GetBoneSocket("tongue").position;
        //     // var mtStartRot = portrait.GetBoneSocket("mouth (top)").localRotation.eulerAngles.z;
        //     // var mbStartRot = portrait.GetBoneSocket("mouth (btm)").localRotation.eulerAngles.z;
        //     // var mtEndRot = mtStartRot - 20;
        //     // var mbEndRot = mbStartRot + 30;

        //     // interpolate between start and target
        //     var t = 0f;
        //     while (t < chompTime)
        //     {
        //         float nt = curve.Evaluate(t / chompTime);
        //         MoveTo(startPos + new Vector3((target.x - startPos.x) * nt, nt * nt * (target.y - startPos.y), target.z));
        //         // portrait.SetBoneRotation("mouth (top)", Mathf.Lerp(mtStartRot, mtEndRot, t), Space.Self);
        //         // portrait.SetBoneRotation("mouth (btm)", Mathf.Lerp(mbStartRot, mbEndRot, t), Space.Self);
        //         t += Time.deltaTime;
        //         yield return null;
        //     }
        //     while (t < chompTime * 2)
        //     {
        //         float nt = curve.Evaluate((t - chompTime) / chompTime);
        //         MoveTo(target + new Vector3((startPos.x - target.x) * nt, nt * nt * (startPos.y - target.y), startPos.z));
        //         // portrait.SetBoneRotation("mouth (top)", Mathf.Lerp(mtEndRot, mtStartRot, t - 1), Space.Self);
        //         // portrait.SetBoneRotation("mouth (btm)", Mathf.Lerp(mbEndRot, mbStartRot, t - 1), Space.Self);
        //         t += Time.deltaTime;
        //         yield return null;
        //     }
        //     // yield return new WaitForSeconds(0.1f);
        //     // TransitionToState(State.MouthClose.ToString());
        //     // yield return new WaitForSeconds(0.15f);
        //     MoveTo(startPos);
        //     TransitionToState(State.Idle.ToString());
        // }

        protected override void OnEnable()
        {
            _playDeathAnim = () => TransitionToState(State.Death.ToString());
            base.OnEnable();
        }

        public Vector3 TonguePos => portrait.GetBoneSocket("tongue").position;
        public Vector3 Neck1Pos => portrait.GetBoneSocket("neck 1").position;
        public Vector3 Neck2Pos => portrait.GetBoneSocket("neck 2").position;
        public Vector3 Neck3Pos => portrait.GetBoneSocket("neck 3").position;
        public Vector3 HeadPos => portrait.GetBoneSocket("head (mid)").position;

        public void MoveTo(Vector3 position)
        {
            if (tongue == null) tongue = portrait.GetBone("tongue");
            portrait.SetBoneIK(tongue, position, Space.World);
        }
    }
}