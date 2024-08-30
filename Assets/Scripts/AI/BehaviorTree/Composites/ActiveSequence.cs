using System.Collections.Generic;

namespace BehaviorTree
{
    public class ActiveSequence : Sequence
    {
        protected bool _startedFromInactive = false;
        public bool Restarted { get; private set; } = false;
        protected Node _prevChild;
        public ActiveSequence(List<Node> children) : base(children) { }

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
            if (childState == State.FAILURE)
            {
                State = State.FAILURE;
                return;
            }

            if (childState == State.SUCCESS)
            {
                if (_currChildInd + 1 < Children.Count)
                {
                    // Do not want to push a child in running state onto the scheduler to have it ticked again
                    if (!Restarted || (Restarted && _prevChild != Children[_currChildInd + 1]))
                        Tree.Scheduled.AddFirst(Children[++_currChildInd]);
                    else ++_currChildInd;
                    return;
                }
                else State = State.SUCCESS;
            }
        }

        // INVARIANT: Restarted is false end of each turn
        public override State Tick()
        {
            if (State == State.SUSPENDED) Done();
            if (State == State.INACTIVE)
            {
                _startedFromInactive = true;
                OnInit();
            }
            if (State == State.RUNNING) // if final status acquired no running children require abort
            {
                if (_startedFromInactive)
                {
                    _startedFromInactive = false;
                    return State;
                }

                if (!Restarted)
                {
                    Restarted = true;
                    _prevChild = Children[_currChildInd];
                    if (_currChildInd != 0) OnInit();
                }
                else if (_prevChild != Children[_currChildInd])
                {
                    _prevChild.Abort();
                    Restarted = false;
                }
            }
            else if (Restarted)
            {
                // Success means prevChild have alse been executed this turn and succeeded violating at most one tick starting at running state per turn
                // Assert.IsTrue(State == State.FAILURE);
                _prevChild.Abort();
                Restarted = false;
            }

            return State;
        }
    }
}
