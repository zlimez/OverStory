using UnityEngine;

namespace BehaviorTree.SO
{
    public abstract class NodeSO : ScriptableObject
    {
        public string identifier;
        public NodeSO Parent;
    }
}
