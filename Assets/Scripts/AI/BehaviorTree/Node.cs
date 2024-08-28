using System.Collections.Generic;
using UnityEngine.Assertions;

namespace BehaviorTree
{
    public enum State
    {
        INACTIVE,
        RUNNING,
        SUCCESS,
        FAILURE,
        SUSPENDED
    }

    public abstract class Node
    {
        public State State { get; protected set; } = State.INACTIVE;
        public BT Tree;
        // 1st tick triggers push of child to front of Scheduled, 2nd tick is final all children must have been processed state determined, used to cater for first turn
        public Node Parent;

        public virtual State Tick()
        {
            if (State == State.SUSPENDED) Done();
            if (State == State.INACTIVE) OnInit();
            return State;
        }

        public virtual void Setup(BT tree)
        {
            Tree = tree;
        }

        protected virtual void OnInit()
        {
            State = State.RUNNING;
        }

        // Cleanup resources here
        public virtual void Done()
        {
            State = State.INACTIVE;
        }

        public virtual void Abort()
        {
            Assert.IsTrue(State == State.RUNNING);
            Assert.IsTrue(this is not Root);
            State = State.SUSPENDED;
        }

        public bool Completed => State == State.SUCCESS || State == State.FAILURE;

        public virtual void OnChildComplete(Node child, State childState)
        {
            child.Done();
        }

        public abstract List<Node> GetChildren();
    }
}
