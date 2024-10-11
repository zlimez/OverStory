using System;
using System.Collections.Generic;
using UnityEngine;
using Algorithms;
using UnityEngine.Assertions;

namespace BehaviorTree
{
    // TODO: Consider Composition over Inheritance
    public class ProbSelector : Selector
    {
        // NOTE: User's responsibility to ensure the order of probFactor key match args in a probFunc and order of probFuncs match children
        string[][] _probFuncsFactors;
        Func<List<object>, float>[] _probFuncs; // Does not nececessarily sum to 1, relative weightage respected

        public ProbSelector(List<Node> children, Func<List<object>, float>[] probFuncs, string[][] proFuncsFactors) : base(children)
        {
            Assert.AreEqual(children.Count, probFuncs.Length);
            Assert.AreEqual(children.Count, proFuncsFactors.Length);
            _probFuncsFactors = proFuncsFactors;
            _probFuncs = probFuncs;
        }

        private void ShuffleChildren()
        {
            List<float> probDist = new();
            for (int i = 0; i < Children.Count; i++) probDist.Add(_probFuncs[i](Tree.GetData(_probFuncsFactors[i])));

            var probDebug = "";
            foreach (var prob in probDist) probDebug += prob + ", ";
            Debug.Log("Probabilities: " + probDebug);
            var order = ListUtils.WeightedShuffle(probDist, Children);
            var shuffledProbFuncs = new Func<List<object>, float>[Children.Count];
            var shuffledProbFuncsFactors = new string[Children.Count][];
            for (int i = 0; i < order.Count; i++)
            {
                shuffledProbFuncs[i] = _probFuncs[order[i]];
                shuffledProbFuncsFactors[i] = _probFuncsFactors[order[i]];
            }
            _probFuncs = shuffledProbFuncs;
            _probFuncsFactors = shuffledProbFuncsFactors;
        }

        protected override void OnInit()
        {
            ShuffleChildren();
            base.OnInit();
        }
    }
}
