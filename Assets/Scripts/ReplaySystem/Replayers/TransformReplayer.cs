using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;

namespace DeepBreath.ReplaySystem {
    public class TransformReplayer : MonoBehaviour, Replayer<Pair<Vector3, Quaternion>>
    {
        public void Consume(ActionReplayRecord<Pair<Vector3, Quaternion>> transformRecord) {
            var transformData = transformRecord.GetData();
            transform.position = transformData.head;
            transform.rotation = transformData.tail;
        }
    }
}
