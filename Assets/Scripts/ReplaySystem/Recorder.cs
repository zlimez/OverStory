using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepBreath.ReplaySystem {
    public interface Recorder<T>
    {
        public ActionReplayRecord<T> ProduceRecord();
    }
}
