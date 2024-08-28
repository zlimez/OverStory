using System.Collections.Generic;

namespace BehaviorTree
{
    public class Action : Node
    {
        public override List<Node> GetChildren()
        {
            return null;
        }
    }
}