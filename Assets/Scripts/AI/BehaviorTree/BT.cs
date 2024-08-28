using System.Collections.Generic;
using DataStructures;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorTree
{
    public class BT
    {
        protected Node _root;
        public readonly Deque<Node> Scheduled = new();
        // NOTE: Directly listening to blackboard events prohibited to prevent mid tick changes and multiple reevaluates per composite,
        // a buffer is used to store events and then processed at the start of the next tick
        public Dictionary<string, UnityEvent> TrackedEvents = new(); // One event string should be from one blackboard
        // Write from sources are done to back buffer, front buffer is processed at the start of next tick
        private Queue<UnityEvent> _eventFrontBuffer = new(), _eventBackBuffer = new();
        private Queue<UnityEvent> _eventWriteBuffer;
        private List<Blackboard> _blackboards;
        public Dictionary<string, object> HeadBlackboard { get; private set; } = new();

        public BT(Node root, List<Blackboard> blackboards)
        {
            Setup(root);
            _root = root;
            _blackboards = blackboards;
            _eventWriteBuffer = _eventBackBuffer;
        }

        void Setup(Node root)
        {
            Queue<Node> q = new();
            q.Enqueue(root);
            while (q.Count > 0)
            {
                Node node = q.Dequeue();
                node.Setup(this);
                var children = node.GetChildren();
                if (children == null) continue;
                foreach (var child in children)
                    q.Enqueue(child);
            }

            foreach (var observedData in HeadBlackboard.Keys)
            {
                bool sourceFound = false;
                foreach (var blackboard in _blackboards)
                {
                    if (blackboard.DataEvents.ContainsKey(observedData))
                    {
                        sourceFound = true;
                        blackboard.DataEvents[observedData].AddListener((obj) =>
                        {
                            HeadBlackboard[observedData] = obj;
                            _eventWriteBuffer.Enqueue(TrackedEvents[observedData]);
                        });
                        break;
                    }
                }
                if (!sourceFound) throw new UnityException($"No source found for {observedData}");
            }
        }

        public State Tick()
        {
            foreach (var e in _eventFrontBuffer) e.Invoke();
            _eventFrontBuffer.Clear();
            (_eventFrontBuffer, _eventBackBuffer) = (_eventBackBuffer, _eventFrontBuffer);
            _eventWriteBuffer = _eventBackBuffer;

            if (Scheduled.Count == 0) Scheduled.PushFront(_root);
            Scheduled.PushBack(null); // End of turn marker, last element should be root
            while (Step()) ;
            State execState = _root.State;
            if (execState != State.RUNNING) _root.Done();
            return execState;
        }

        // TODO: For observed var change that causes reevaluation and potentially aborts starting from parent will be more efficient
        // from children many alt routes in children subtree may be taken only for parent to restart and abort the alt path
        bool Step()
        {
            Node node = Scheduled.PeekFront();
            if (node == null) return false;

            State ogState = node.State;
            State state = node.Tick();
            // Non leaf control nodes with children first traversed, child node pushed to Scheduled, do not pop this node until children evaluated (Action node always popped)
            // Active controls nodes that require condition recheck every turn do not pop until recheck is done (Reevaluated toggles back to false)
            if ((node is ActiveSelector selector && selector.Restarted)
                || (node is ActiveSequence sequence && sequence.Restarted)
                || (node is ObserveSelector observeSelector && observeSelector.Restarted)
                || (node is ObserveSequence observeSequence && observeSequence.Restarted)
            ) return true;
            if (node is not Action && ogState == State.INACTIVE) return true;

            Scheduled.PopFront();
            // INVARIANT: End of each turn, only running nodes / running but aborted by parent are in Scheduled 
            if (state == State.RUNNING)
                Scheduled.PushBack(node);
            else if (node.Completed) node.Parent?.OnChildComplete(node, state);

            return true;
        }

        public List<object> GetData(List<string> name)
        {
            List<object> data = new();
            foreach (var n in name)
                data.Add(HeadBlackboard[n]);
            return data;
        }
    }

}