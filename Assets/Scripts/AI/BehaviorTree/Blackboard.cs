using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine.Events;

namespace BehaviorTree
{
    // Connected to EventManager to listen for events that changes data and retrieve from respective systems / data containers
    public class Blackboard
    {
        readonly Dictionary<string, (UnityEvent<object>, UnityAction<object>)> dataEvents = new();

        public Blackboard(Pair<string, string>[] varEvents)
        {
            foreach (var varEvent in varEvents)
            {
                var varName = varEvent.Head;
                var eventName = varEvent.Tail;

                dataEvents[varName] = (new UnityEvent<object>(), (obj) => dataEvents[varEvent.Head].Item1.Invoke(obj));
                EventManager.StartListening(new GameEvent(eventName), dataEvents[varName].Item2);
            }
        }

        public bool IsTracking(string varName) => dataEvents.ContainsKey(varName);
        public void AddListener(string varName, UnityAction<object> func) => dataEvents[varName].Item1.AddListener(func);
        public void RemoveListener(string varName, UnityAction<object> func) => dataEvents[varName].Item1.RemoveListener(func);


        public void Teardown()
        {
            foreach (var dataEvent in dataEvents)
                EventManager.StopListening(new GameEvent(dataEvent.Key), dataEvent.Value.Item2);
        }
    }
}
