using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Composite
    {
        protected int _currChildInd;

        public Sequence(List<Node> children) : base(children) {}

        public override void OnChildComplete(Node child, State childState)
        {
            child.Done();
            if (childState == State.FAILURE)
            {
                State = State.FAILURE;
                return;
            }

            if (childState == State.SUCCESS)
            {
                if (_currChildInd + 1 < Children.Count)
                {
                    Tree.Scheduled.AddFirst(Children[++_currChildInd]);
                    return;
                }
                else State = State.SUCCESS;
            }
        }

        protected override void OnInit()
        {
            _currChildInd = 0;
            Tree.Scheduled.AddFirst(Children[_currChildInd]);
            base.OnInit();
        }

        public override void Abort()
        {
            base.Abort();
            Children[_currChildInd].Abort();
        }
    }
}
