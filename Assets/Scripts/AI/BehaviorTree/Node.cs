using System.Collections.Generic;

namespace BehaviorTree
{
    public enum State
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public abstract class Node
    {
        protected State state;

        public Node parent;
        protected List<Node> children = new();
        private Dictionary<string, object> _context;

        public Node(Dictionary<string, object> context)
        {
            _context = context;
            parent = null;
        }

        public Node(List<Node> children, Dictionary<string, object> context)
        {
            _context = context;
            foreach (Node child in children)
                Attach(child);
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public abstract State Evaluate();
        private void SetData(string name, object val) {
            if (_context.ContainsKey(name)) {
                _context[name] = val;
            } else _context.Add(name, val);
        }

        private T GetData<T>(string name) { return (T)_context[name]; }
    }
}
