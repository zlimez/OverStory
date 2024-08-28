using System.Collections.Generic;
using UnityEngine.Assertions;

namespace BehaviorTree
{
    public class Repeater : Decorator
    {
        int _times;
        int _timesExecuted = 0;
        List<State> _repeatedStates = new();

        public Repeater(Node child, int times) : base(child)
        {
            Assert.IsTrue(times > 0);
            _times = times;
        }

        public override void OnChildComplete(Node child, State childState)
        {
            _repeatedStates.Add(childState);
            if (++_timesExecuted == _times)
            {
                if (_repeatedStates.Contains(State.FAILURE))
                    State = State.FAILURE;
                else State = State.SUCCESS;

            }
            else Tree.Scheduled.PushFront(Child);
            base.OnChildComplete(child, childState);
        }

        protected override void OnInit()
        {
            _timesExecuted = 0;
            _repeatedStates.Clear();
            base.OnInit();
        }
    }
}
