using System.Collections.Generic;
using Abyss.Environment.Enemy;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class MonoBT : MonoBehaviour
    {
        protected BT _bT;
        protected readonly List<Blackboard> _bbs = new(); // local blackboards for this instance

#if UNITY_EDITOR
        protected virtual void OnEnable() => Setup();
        protected virtual void OnDisable() => StopBT();
#endif

        public abstract void Setup();

        protected void StopBT()
        {
            _bT?.Teardown();
            _bT = null;
        }

        void Update() => _bT?.Tick();

        protected virtual void OnDestroy()
        {
            StopBT();
            foreach (var _bb in _bbs) _bb.Teardown();
        }

        public EnemyAttr NormalizeEnemyAttr(EnemyAttr attr)
        {
            EnemyAttr normalizedAttr = new();
            // speed 
            if (attr.speed >= 5) normalizedAttr.speed = 1 + (attr.speed - 5) * 0.2f;
            else normalizedAttr.speed = 1 + (attr.speed - 5) * 0.1f;
            // alertness
            if (attr.alertness >= 5) normalizedAttr.alertness = 1 + (attr.alertness - 5) * 0.2f;
            else normalizedAttr.alertness = 1 + (attr.alertness - 5) * 0.1f;
            // friendliness
            normalizedAttr.friendliness = attr.friendliness / 10f;
            return normalizedAttr;
        }
    }
}
