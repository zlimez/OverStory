namespace BehaviorTree
{
    public class Failer : Decorator
    {
        public Failer(Node child) : base(child) {}

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
            State = State.FAILURE;
        }
    }
}
