using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;

namespace DeepBreath.ReplaySystem {
    public class AnimReplayer : MonoBehaviour, Replayer<Pair<string, string>>
    {
        [SerializeField] private Animator animator;

        public void Consume(ActionReplayRecord<Pair<string, string>> record) {
            AnimReplayRecord animRecord = (AnimReplayRecord)record;
            if (animRecord.IsNoChange) return;
            var animData = animRecord.GetData();
            animator.SetBool(animData.head, bool.Parse(animData.tail));
        }
    }
}
