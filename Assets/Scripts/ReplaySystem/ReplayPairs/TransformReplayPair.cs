using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;

namespace DeepBreath.ReplaySystem {
    public class TransformReplayPair : ActionReplayPair<TransformRecorder, TransformReplayer, Pair<Vector3, Quaternion>> {}
}
