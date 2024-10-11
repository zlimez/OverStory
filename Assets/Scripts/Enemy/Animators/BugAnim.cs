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
            Land
        }


        protected override void Awake()
        {
            base.Awake();
            currState = State.Idle.ToString();
        }
    }
}