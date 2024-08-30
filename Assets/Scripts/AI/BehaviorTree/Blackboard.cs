using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine.Events;

namespace BehaviorTree
{
    // Connected to EventManager to listen for events that changes data and retrieve from respective systems / data containers
    public class Blackboard
    {
        public Dictionary<string, UnityEvent<object>> DataEvents { get; private set; }

        public Blackboard(Pair<string, string>[] varEvents)
        {
            foreach (var varEvent in varEvents)
            {
                var varName = varEvent.Head;
                var eventName = varEvent.Tail;

                DataEvents[varName] = new UnityEvent<object>();
                EventManager.StartListening(new GameEvent(eventName), (obj) =>
                {
                    DataEvents[varEvent.Head].Invoke(obj);
                });
            }
        }
    }
}
