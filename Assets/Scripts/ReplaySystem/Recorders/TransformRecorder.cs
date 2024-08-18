using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;

namespace DeepBreath.ReplaySystem {
    public class TransformRecorder : MonoBehaviour, Recorder<Pair<Vector3, Quaternion>>
    {
        public ActionReplayRecord<Pair<Vector3, Quaternion>> ProduceRecord() {
            var record = new TransformReplayRecord(transform.position, transform.rotation);
            return record;
        }
    }
}
