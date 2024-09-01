using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.SO
{
    [CreateAssetMenu(fileName = "New Composite", menuName = "BehaviorTree/Composite")]
    public class CompositeSO : NodeSO
    {
        public enum CompositeType { Sequence, Selector, ActiveSequence, ActiveSelector, ProbSelector }
        public CompositeType type;
        public List<NodeSO> children;
    }
}
