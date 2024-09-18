using System.Collections.Generic;
using Tuples;
using UnityEngine;
using UnityEngine.Events;

// TODO: Remove reliance on Unity
namespace BehaviorTree
{
    public class BT
    {
        // For debugging
        private int _idCounter = 0;

        protected Node _root;
        public readonly LinkedList<Node> Scheduled = new();
        // NOTE: Directly subscription to blackboard events prohibited to prevent mid tick changes and multiple reevaluates per composite,
        // a buffer is used to store events and then processed at the start of the next tick
        readonly Dictionary<string, UnityEvent> _trackedVars = new();
        Queue<UnityEvent> _eventFrontBuffer = new(), _eventBackBuffer = new();
        Queue<UnityEvent> _eventWriteBuffer;
        readonly Blackboard[] _blackboards; // By contract should possess or have reference to vars that are tracked by this BT whose changes can cause composite restarts
        readonly Dictionary<string, object> _headBoard = new();

        public BT(Node firstNode, Pair<string, object>[] charParams, Blackboard[] blackboards = null)
        {
            _root = new Root(firstNode);
            _blackboards = blackboards;
            _eventWriteBuffer = _eventBackBuffer;
            foreach (var charParam in charParams) _headBoard[charParam.Head] = charParam.Tail;
            Setup();
        }

        void Setup()
        {
            Queue<Node> q = new();
            q.Enqueue(_root);
            while (q.Count > 0)
            {
                Node node = q.Dequeue();
                node.Id = _idCounter++;
                node.Setup(this);
                var children = node.GetChildren();
                if (children == null) continue;
                foreach (var child in children)
                    q.Enqueue(child);
            }

            if (_blackboards == null && _trackedVars.Count > 0) throw new UnityException($"No blackboard configured to be observed");
            foreach (var varTEvent in _trackedVars)
            {
                var varName = varTEvent.Key;
                var changeEvent = varTEvent.Value;
                bool sourceFound = false;
                foreach (var blackboard in _blackboards)
                {
                    if (blackboard.DataEvents.ContainsKey(varName))
                    {
                        sourceFound = true;
                        blackboard.DataEvents[varName].AddListener((obj) =>
                        {
                            _headBoard[varName] = obj;
                            _eventWriteBuffer.Enqueue(changeEvent);
                        });
                        break;
                    }
                }
                if (!sourceFound) throw new UnityException($"No source found for tracked {varName}");
            }
        }

        public void Teardown() {
            // TODO: Remove all listeners
        }

        public void AddTracker(string observedVar, UnityAction func)
        {
            if (!_trackedVars.ContainsKey(observedVar))
                _trackedVars[observedVar] = new();
            _trackedVars[observedVar].AddListener(func);
        }

        public State Tick()
        {
            foreach (var e in _eventFrontBuffer) e.Invoke();
            _eventFrontBuffer.Clear();
            (_eventFrontBuffer, _eventBackBuffer) = (_eventBackBuffer, _eventFrontBuffer);
            _eventWriteBuffer = _eventBackBuffer;

            if (Scheduled.Count == 0) Scheduled.AddFirst(_root);
            Scheduled.AddLast((Node)null); // End of turn marker, last element should be root
            while (Step()) ;
            Scheduled.RemoveFirst(); // Remove null marker

            State execState = _root.State;
            if (execState != State.RUNNING) _root.Done();
            // Debug.Log($"Executed with state {execState}");
            return execState;
        }

        // TODO: For observed var change that causes reevaluation and potentially aborts starting from parent will be more efficient
        // from children many alt routes in children subtree may be taken only for parent to restart and abort the alt path
        bool Step()
        {
            Node node = Scheduled.First.Value;
            if (node == null) return false;
            // Debug.Log($"Processing {node.Id}");

            State ogState = node.State;
            State state = node.Tick();
            // Non leaf control nodes with children first traversed, child node pushed to Scheduled, do not pop this node until children evaluated (Action node always popped)
            // Active controls nodes that require condition recheck every turn do not pop until recheck is done (Reevaluated toggles back to false)
            if ((node is ActiveSelector selector && selector.Restarted)
                || (node is ActiveSequence sequence && sequence.Restarted)
                || (node is ObserveSelector observeSelector && observeSelector.Restarted)
                || (node is ObserveSequence observeSequence && observeSequence.Restarted)
            ) return true;
            if (ogState == State.INACTIVE) return true;

            Scheduled.RemoveFirst();
            // Debug.Log($"Popped {node.Id}");
            // INVARIANT: End of each turn, only running nodes / running but aborted by parent are in Scheduled 
            if (state == State.RUNNING)
            {
                Scheduled.AddLast(node);
                // Debug.Log($"Pushed {node.Id}");
            }
            else if (node.Completed) node.Parent?.OnChildComplete(node, state);

            return true;
        }

        public List<object> GetData(string[] names)
        {
            List<object> data = new();
            foreach (var n in names)
            {
                if (!_headBoard.ContainsKey(n)) throw new UnityException($"Var {n} not found in board");
                data.Add(_headBoard[n]);
            }
            return data;
        }

        public T GetDatum<T>(string name, bool nullable = false)
        {
            if (!_headBoard.ContainsKey(name)) {
                if (nullable) return default;
                throw new UnityException($"Var {name} not found in board");
            }
            return (T)_headBoard[name];
        }

        public void SetDatum(string name, object val)
        {
            if (!_headBoard.ContainsKey(name)) _headBoard.Add(name, val);
            else _headBoard[name] = val;
        }

        public void ClearDatum(string name) { _headBoard[name] = null; }
    }
}