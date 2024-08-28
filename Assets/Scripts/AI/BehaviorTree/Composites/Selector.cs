using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Composite
    {
        protected int _currChildInd;

        public Selector(List<Node> children) : base(children) { }

        public override void OnChildComplete(Node child, State childState)
        {
            if (childState == State.SUCCESS)
            {
                State = State.SUCCESS;
                return;
            }

            if (childState == State.FAILURE)
            {
                if (_currChildInd + 1 < Children.Count)
                {
                    Tree.Scheduled.PushFront(Children[++_currChildInd]);
                    return;
                }
                else State = State.FAILURE;
            }
            base.OnChildComplete(child, childState);
        }

        protected override void OnInit()
        {
            _currChildInd = 0;
            Tree.Scheduled.PushFront(Children[_currChildInd]);
            base.OnInit();
        }

        public override void Abort()
        {
            base.Abort();
            Children[_currChildInd].Abort();
        }
    }
}
