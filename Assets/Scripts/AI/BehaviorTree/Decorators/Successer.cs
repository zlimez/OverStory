namespace BehaviorTree
{
    public class Successer : Decorator
    {
        public Successer(Node child) : base(child) { }

        public override void OnChildComplete(Node child, State childState)
        {
            State = State.SUCCESS;
            base.OnChildComplete(child, childState);
        }
    }
}

