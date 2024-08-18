using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;

namespace DeepBreath.ReplaySystem {
    public interface ActionReplayRecord<T> {
        public T GetData();
    }

    public struct TransformReplayRecord : ActionReplayRecord<Pair<Vector3, Quaternion>> {
        private Vector3 deltaPosition;

        private Quaternion rotation;

        public TransformReplayRecord(Vector3 deltaPosition, Quaternion rotation) {
            this.deltaPosition = deltaPosition;
            this.rotation = rotation;
        }

        public Pair<Vector3, Quaternion> GetData() {
            return new Pair<Vector3, Quaternion>(deltaPosition, rotation);
        }
    }

    // NOTE: Supports only bool type transitions
    public struct AnimReplayRecord : ActionReplayRecord<Pair<string, string>> 
    {
        public readonly static string NoChange = "NO CHANGE";
        private string varName;
        private string transition;

        public bool IsNoChange => transition.Equals(NoChange);

        public AnimReplayRecord(string varName, string transition) {
            this.varName = varName;
            this.transition = transition;
        }

        public Pair<string, string> GetData() {
            return new Pair<string, string>(varName, transition);
        }
    }

    public struct SpriteFlipReplayRecord : ActionReplayRecord<string> 
    {
        public readonly static string NoChange = "NO CHANGE";
        private string transition;

        public bool IsNoChange => transition.Equals(NoChange);

        public SpriteFlipReplayRecord(string transition) {
            this.transition = transition;
        }

        public string GetData() {
            return transition;
        }
    }
}