namespace BehaviorTree
{
    public class Inverter : Decorator
    {
        public Inverter(Node child) : base(child) {}

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
            State = childState == State.SUCCESS ? State.FAILURE : State.SUCCESS;
        }
    }
}
