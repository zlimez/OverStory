// Root must have at least one child
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace BehaviorTree {
    public class Root : Node
    {
        public Node Child { get; private set; }
        public Root(Node child)
        {
#if DEBUG
            Assert.IsNull(Parent);
            Assert.IsNotNull(child);
#endif
            Child = child;
            Child.Parent = this;
        }

        public override void OnChildComplete(Node child, State childState) { 
            State = childState;
            base.OnChildComplete(child, childState);
        }

        protected override void OnInit()
        {
            Tree.Scheduled.AddFirst(Child);
            base.OnInit();
        }

        public override List<Node> GetChildren()
        {
            return new List<Node> { Child };
        }
    }
}
