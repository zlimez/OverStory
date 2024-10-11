using System.Collections.Generic;
using UnityEngine;

// DO NOT USE
namespace BehaviorTree.Actions
{
    public class Attack : CfAction
    {
        // private Animator _animator;
        private Transform _lastTarget;
        private Health _enemyHealth;

        private int _hitpoints;
        private float _attackTime;
        private float _attackCounter = 0;

        public Attack(string[] _params) : base(_params) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _attackTime = (float)dataRef[0];
            _hitpoints = (int)dataRef[1];
        }

        public override void Update()
        {
            Transform target = Tree.GetDatum<Transform>("target", true);
            if (target != _lastTarget)
            {
                _enemyHealth = target.GetComponent<Health>();
                _lastTarget = target;
                _attackCounter = 0;
            }

            _attackCounter += Time.deltaTime;
            if (_attackCounter >= _attackTime)
            {
                bool enemyIsDead = _enemyHealth.TakeHit(_hitpoints);
                if (enemyIsDead)
                {
                    State = State.SUCCESS;
                    Tree.ClearDatum("target");
                }
                _attackCounter = 0f;
            }
        }
    }
}
