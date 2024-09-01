using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class Action : Node
    {
        public override State Tick()
        {
            if (State == State.SUSPENDED) Done();
            else if (State == State.INACTIVE) OnInit();
            else if (State == State.RUNNING) Update();
            return State;
        }
        public abstract void Update();
        public override List<Node> GetChildren() { return null; }
    }

    // Actions with init params
    public abstract class CfAction : Action {
        protected string[] _params;
        public CfAction(string[] parameters) { _params = parameters; }
    }
}