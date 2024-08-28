using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviorTree {
    // Can be connected to EventManager to listen for events that changes data and retrieve from respective systems / data containers
    public class Blackboard
    {
        // Updates data first then invokes event
        public Dictionary<string, object> Data { get; private set; }
        public Dictionary<string, UnityEvent<object>> DataEvents { get; private set; }

        // TODO: Prolly best for computed data with dependencies on other system data
        void UpdateData(string key, object value)
        {
            Data[key] = value; // Some compute function here that can be passed in constructor
            DataEvents[key].Invoke(value);
        }

        // Data exists in other systems simply pass the data to subscribers -> Invoked through EventManager in that system
        void PassData(string key, object value)
        {
            DataEvents[key].Invoke(value);
        }
    }
}
