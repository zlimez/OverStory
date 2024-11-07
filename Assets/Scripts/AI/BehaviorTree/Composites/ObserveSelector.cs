using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace BehaviorTree
{
    public class ObserveSelector : Selector
    {
        public bool Restarted { get; private set; } = false;
        Node _prevChild;
        readonly string[] _observedVars;
        bool _shouldReevaluate = false;
        readonly Func<object, bool> _restartCondition;

        void PromptReevaluate()
        {
            if (State == State.RUNNING) _shouldReevaluate = true;
        }

        public ObserveSelector(List<Node> children, string[] observedVars, Func<object, bool> restartCondition) : base(children)
        {
            _observedVars = observedVars;
            _restartCondition = restartCondition;
        }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            foreach (var observedVar in _observedVars)
                Tree.AddTracker(observedVar, PromptReevaluate);
        }

        public override void Teardown()
        {
            foreach (var observedVar in _observedVars)
                Tree.AddTracker(observedVar, PromptReevaluate);
        }

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
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
                        Tree.Scheduled.AddFirst(Children[++_currChildInd]);
                    else ++_currChildInd;
                    return;
                }
                else State = State.FAILURE;
            }
        }

        public override State Tick()
        {
            if (State == State.SUSPENDED)
            {
                Done();
            }
            else if (State == State.INACTIVE)
            {
                OnInit();
            }
            else if (State == State.RUNNING)
            {
                if (_shouldReevaluate)
                {
                    _shouldReevaluate = false;
                    Restarted = _restartCondition(Tree.GetData(_observedVars, true)); // nullable set to true as one or more env variable might not have changed the first time to be registed in headboard
                    if (Restarted)
                    {
                        _prevChild = Children[_currChildInd];
                        if (_currChildInd != 0)
                            OnInit();
                        else Restarted = false;
                    }
                }
                else
                {
                    if (Restarted && _prevChild != Children[_currChildInd]) _prevChild.Abort();
                    Restarted = false;
                }
            }
            else if (Restarted)
            {
                // Failure means prevChild have also been executed this turn and failed violating at most one tick starting at running state per turn
#if UNITY_EDITOR
                Assert.IsTrue(State == State.SUCCESS);
#endif
                _prevChild.Abort();
                Restarted = false;
            }

            return State;
        }
    }
}
