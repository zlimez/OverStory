using UnityEngine;

namespace Abyss.Environment.Enemy.Anim
{
    public class BloomAnim : PortraitAnim
    {
        public enum State { Idle, Death, Wiggle, MouthOpen }

        AnyPortrait.apOptBone tongue;

        protected override void Awake()
        {
            base.Awake();
            currState = State.Idle.ToString();
        }

        protected override void OnEnable()
        {
            _playDefeatAnim = () => TransitionToState(State.Death.ToString());
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