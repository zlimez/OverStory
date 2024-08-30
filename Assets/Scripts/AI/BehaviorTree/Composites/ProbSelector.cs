using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using UnityEngine.Assertions;

namespace BehaviorTree {
    // TODO: Consider Composition over Inheritance
    public class ProbSelector : Selector
    {
        // NOTE: User's responsibility to ensure the order of probFactor key match args in a probFunc and order of probFuncs match children
        readonly List<List<string>> _probFuncsFactors;
        readonly List<Func<List<object>, float>> _probFuncs; // Does not nececessarily sum to 1, relative weightage respected

        public ProbSelector(List<Node> children, List<Func<List<object>, float>> probFuncs, List<List<string>> proFuncsFactors) : base(children)
        {
            Assert.AreEqual(children.Count, probFuncs.Count);
            Assert.AreEqual(children.Count, proFuncsFactors.Count);
            _probFuncsFactors = proFuncsFactors;
            _probFuncs = probFuncs;
        }

        private void ShuffleChildren()
        {
            List<float> probDist = new();
            for (int i = 0; i < Children.Count; i++) probDist.Add(_probFuncs[i](Tree.GetData(_probFuncsFactors[i])));
            ListUtils.WeightedShuffle(probDist, Children);
        }

        protected override void OnInit()
        {
            ShuffleChildren();
            base.OnInit();
        }
    }
}
