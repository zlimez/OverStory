using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace BehaviorTree
{
    public class ObserveSelector : Selector
    {
        public bool Restarted { get; private set; } = false;
        Node _prevChild;
        readonly List<string> _observedVals;
        bool _shouldReevaluate = false;
        readonly Func<object, bool> _restartCondition;

        public ObserveSelector(List<Node> children, List<string> observedVals) : base(children)
        {
            _observedVals = observedVals;
        }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            foreach (var observedVal in _observedVals)
            {
                if (!Tree.TrackedEvents.ContainsKey(observedVal))
                    Tree.TrackedEvents[observedVal] = new UnityEvent();
                Tree.TrackedEvents[observedVal].AddListener(() =>
                {
                    if (State == State.RUNNING) _shouldReevaluate = true;
                });
            }
        }

        public override void OnChildComplete(Node child, State childState)
        {
            if (childState == State.SUCCESS)
            {
                State = State.SUCCESS;
                return;
            }

            if (childState == State.FAILURE)
            {
                if (_currChildInd + 1 < Children.Count)
                {
                    // Do not want to push a child in running state onto the scheduler to have it ticked again
                    if (!Restarted || (Restarted && _prevChild != Children[_currChildInd + 1]))
                        Tree.Scheduled.PushFront(Children[++_currChildInd]);
                    else ++_currChildInd;
                    return;
                }
                else State = State.FAILURE;
            }
            child.Done();
        }

        public override State Tick()
        {
            if (State == State.SUSPENDED) Done();
            if (State == State.INACTIVE) OnInit();
            if (State == State.RUNNING)
            {
                if (_shouldReevaluate)
                {
                    _shouldReevaluate = false;
                    Restarted = _restartCondition(Tree.GetData(_observedVals));
                    if (Restarted)
                    {
                        _prevChild = Children[_currChildInd];
                        if (_currChildInd != 0) OnInit();
                    }
                }
                else if (Restarted && _prevChild != Children[_currChildInd])
                {
                    _prevChild.Abort();
                    Restarted = false;
                }

            }
            else if (Restarted) {
                // Failure means prevChild have alse been executed this turn and failed violating at most one tick starting at running state per turn
                Assert.IsTrue(State == State.SUCCESS);
                _prevChild.Abort();
                Restarted = false;
            }

            return State;
        }
    }
}
