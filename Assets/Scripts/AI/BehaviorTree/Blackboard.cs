using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;

namespace BehaviorTree
{
    /// <summary>
    /// Connected to EventManager to listen for events that changes data and retrieve from respective systems / data containers.
    /// Propagates the changes to the headboard of connected BTs when variable names match.
    /// </summary>
    public class Blackboard
    {
        readonly Dictionary<string, RefPair<Action<object>, Action<object>>> dataEvents = new();

        /// <summary>
        /// Head of pair is match the variable name stored in head board of intended BTs for changes to be propagated to the BT. tail is the events that will trigger the update of the variable
        /// </summary>
        /// <param name="varEvents"></param>
        public Blackboard(Pair<string, string[]>[] varEvents)
        {
            foreach (var varEvent in varEvents)
            {
                var varName = varEvent.Head;
                var eventNames = varEvent.Tail;

                if (!dataEvents.ContainsKey(varName)) dataEvents[varName] = new RefPair<Action<object>, Action<object>>(null, (obj) => dataEvents[varName].Head?.Invoke(obj)); // Add the params passed to the invoked event to headboard of connected BTs
                foreach (var eventName in eventNames)
                    EventManager.StartListening(new GameEvent(eventName), dataEvents[varName].Tail);
            }
        }

        public bool IsTracking(string varName) => dataEvents.ContainsKey(varName);
        public void AddListener(string varName, Action<object> func) => dataEvents[varName].Head += func;
        public void RemoveListener(string varName, Action<object> func) => dataEvents[varName].Head -= func;


        public void Teardown()
        {
            foreach (var dataEvent in dataEvents)
                EventManager.StopListening(new GameEvent(dataEvent.Key), dataEvent.Value.Tail);
        }
    }
}
