using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abyss.Environment.Enemy.Anim
{
    public class BugAnim : PortraitAnim
    {
        public enum State
        {
            Idle,
            Charge,
            Jump,
            Drop,
            Land,
            Death
        }


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
    }
}