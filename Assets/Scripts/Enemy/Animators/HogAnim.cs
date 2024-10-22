using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.Environment.Enemy.Anim
{
    public class HogAnim : PortraitAnim
    {
        // Polish to add stun animation
        public enum State
        {
            Idle,
            Walk,
            Charge,
            ChargeUp,
            Death,
            Stun,
            Wake,
        }
        [SerializeField] float walkSpeed = 2.5f;
        [SerializeField] float runSpeed = 8f;

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


        public void StateBySpeed(float speed)
        {
#if UNITY_EDITOR
            Assert.IsTrue(speed >= 0);
#endif
            if (speed == 0)
                TransitionToState(State.Idle.ToString());
            else if (speed <= walkSpeed)
                TransitionToState(State.Walk.ToString());
            else
                TransitionToState(State.Charge.ToString());
        }
    }
}
