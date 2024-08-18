using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepBreath.ReplaySystem {
    public interface Replayer<T>
    {
        public void Consume(ActionReplayRecord<T> record);
    }
}

