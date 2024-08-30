namespace BehaviorTree
{
    public class Successer : Decorator
    {
        public Successer(Node child) : base(child) { }

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
            State = State.SUCCESS;
        }
    }
}

