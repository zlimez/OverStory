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
        // For Debugging
        public int Id;

        public State State { get; protected set; } = State.INACTIVE;
        public BT Tree;
        public Node Parent;

        public virtual State Tick()
        {
            if (State == State.SUSPENDED) Done();
            else if (State == State.INACTIVE) OnInit();
            return State;
        }

        public virtual void Setup(BT tree)
        {
            Tree = tree;
        }

        public virtual void Teardown() { }

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
#if DEBUG
            Assert.IsTrue(State == State.RUNNING);
            Assert.IsTrue(this is not Root);
#endif
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
