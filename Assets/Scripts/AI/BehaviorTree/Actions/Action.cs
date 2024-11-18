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

    public abstract class CfAction : Action
    {
        protected string[] _params;
        /// <summary>
        /// paramters: List of variables to retrieve from associated BT's head board. Order of parameters is important should match the inherited classes unpack order in Setup
        /// </summary>
        /// <param name="parameters"></param>
        public CfAction(string[] parameters) { _params = parameters; }
    }
}