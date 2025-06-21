using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace AI.BehaviorTree
{
    /// <summary>
    /// Requires blackboards to be set up with the variables to be observed. Does not "restart" if previous child is the first child, such that no node ticks more that once in running state per turn.
    /// Solution is to attach an observe sequence with the condition checker nesting the original first child as end of the sequence, with same observed vars. Refer to DroneBT
    /// Otherwise can be solved with process all aborts at start of BT tick, changing all children of the restarting node n* nearest to root (in depth) in original "running" branch to abort -> 
    /// no processing when visitedby the scheduler -> then restart with OnInit from n*
    /// </summary>
    public class ObserveSelector : Selector
    {
        public bool WillRestart { get; private set; } = false;
        Node _prevChild;
        readonly string[] _observedVars;
        bool _shouldReevaluate = false;
        readonly Func<List<object>, bool> _restartCondition;

        void PromptReevaluate() { if (State == State.RUNNING) _shouldReevaluate = true; }

        /// <summary>
        /// The object params in restart conditions contains the values of the observed vars retrieved from the headboard
        /// </summary>
        /// <param name="children"></param>
        /// <param name="observedVars"></param>
        /// <param name="restartCondition"></param>
        public ObserveSelector(List<Node> children, string[] observedVars, Func<List<object>, bool> restartCondition) : base(children)
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
                    if (!WillRestart || (WillRestart && _prevChild != Children[_currChildInd + 1]))
                        Tree.Scheduled.AddFirst(Children[++_currChildInd]);
                    else ++_currChildInd;
                }
                else State = State.FAILURE;
            }
        }

        public override State Tick()
        {
            if (State == State.SUSPENDED) Done();
            else if (State == State.INACTIVE) OnInit();
            else if (State == State.RUNNING)
            {
                if (_shouldReevaluate)
                {
                    _shouldReevaluate = false;
                    WillRestart = _restartCondition(Tree.GetData(_observedVars, true)); // nullable set to true as one or more env variable might not have changed the first time to be registed in headboard
                    if (WillRestart)
                    {
                        _prevChild = Children[_currChildInd];
                        if (_currChildInd != 0) OnInit();
                        else WillRestart = false;
                    }
                }
                else
                {
                    if (WillRestart && _prevChild != Children[_currChildInd]) _prevChild.Abort();
                    WillRestart = false;
                }
            }
            else if (WillRestart)
            {
                // Must be success due to after restart the new child succeeded, if failed must mean all inc prevChild executed again when it should be in running
#if UNITY_EDITOR
                Assert.IsTrue(State == State.SUCCESS);
#endif
                _prevChild.Abort();
                WillRestart = false;
            }

            return State;
        }
    }
}
