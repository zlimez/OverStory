using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorTree
{
    public abstract class Composite : Node
    {
        public List<Node> Children { get; private set; }

        public Composite(List<Node> children) {
            Assert.IsNotNull(Parent);
            Children = children;
            foreach (var child in children) child.Parent = this;
        }

        public override List<Node> GetChildren()
        {
            return Children;
        }
    }
}
