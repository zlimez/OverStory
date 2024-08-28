using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms;

namespace BehaviorTree {
    // TODO: Consider Composition over Inheritance
    public class ProbSelector : Selector
    {
        // NOTE: User's responsibility to ensure the order of probFactor key match args in a probFunc and order of probFuncs match children
        readonly List<string> _probFactors;
        readonly List<Func<List<object>, float>> _probFuncs; // Does not nececessarily sum to 1, relative weightage respected

        public ProbSelector(List<Node> children, List<Func<List<object>, float>> probFuncs, List<string> probFactors) : base(children)
        {
            _probFactors = probFactors;
            _probFuncs = probFuncs;
        }

        private void ShuffleChildren()
        {
            List<float> probDist = _probFuncs.Select(f => f(Tree.GetData(_probFactors))).ToList();
            ListUtils.WeightedShuffle(probDist, Children);
        }

        protected override void OnInit()
        {
            ShuffleChildren();
            base.OnInit();
        }
    }
}
