using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class Decorator : Node
    {
        protected Node Child { get; private set; }

        public Decorator(Node child)
        {
            Child = child;
            Child.Parent = this;
        }

        protected override void OnInit()
        {
            Tree.Scheduled.PushFront(Child);
            base.OnInit();
        }

        public override void Abort()
        {
            base.Abort();
            Child.Abort();
        }

        public override List<Node> GetChildren()
        {
            return new List<Node> { Child };
        }
    }
}
