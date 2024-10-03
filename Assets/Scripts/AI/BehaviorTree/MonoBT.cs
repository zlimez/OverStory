using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class MonoBT : MonoBehaviour
    {
        protected BT _bT;
        protected readonly List<Blackboard> _bbs = new(); // local blackboards for this instance

        protected virtual void OnEnable()
        {
            Setup();
        }

        protected virtual void OnDisable()
        {
            StopBT();
        }

        public abstract void Setup();

        protected void StopBT()
        {
            _bT?.Teardown();
            _bT = null;
        }

        void Update()
        {
            _bT?.Tick();
        }

        protected virtual void OnDestroy()
        {
            StopBT();
            foreach (var _bb in _bbs) _bb.Teardown();
        }
    }
}
