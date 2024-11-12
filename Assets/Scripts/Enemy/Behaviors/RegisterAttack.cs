using System;
using BehaviorTree;
using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class RegisterAttack : CfAction
    {
        float _damage;
        bool _hasKnockback;
        EnemyManager _enemyManager;
        string _attackName;

        public RegisterAttack(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _damage = Tree.GetDatum<float>(_params[0]);
            _hasKnockback = Tree.GetDatum<bool>(_params[1]);
            _enemyManager = Tree.GetDatum<EnemyManager>(_params[2]);
            _attackName = _params[3];
        }

        public override void Update() => State = State.SUCCESS;

        protected override void OnInit()
        {
            base.OnInit();
            Action<float> attack = (float str) => Tree.GetDatum<Transform>("target").GetComponent<Player.PlayerManager>().TakeHit(str + _damage, _enemyManager.Specy.specyName, _hasKnockback, _enemyManager.transform.position);
            _enemyManager.OnStrikePlayer += attack;
            Tree.SetDatum(_attackName, attack);
        }
    }

    public class UnregisterAttack : CfAction
    {
        string _attackName;
        EnemyManager _enemyManager;

        public UnregisterAttack(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _enemyManager = Tree.GetDatum<EnemyManager>(_params[0]);
            _attackName = _params[1];
        }

        public override void Update() => State = State.SUCCESS;

        protected override void OnInit()
        {
            base.OnInit();
            _enemyManager.OnStrikePlayer -= Tree.GetDatum<Action<float>>(_attackName);
        }
    }
}
