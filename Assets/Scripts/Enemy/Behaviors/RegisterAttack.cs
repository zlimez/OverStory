using System;
using System.Collections.Generic;
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
            List<object> dataRef = Tree.GetData(_params);
            _damage = (float)dataRef[0];
            _hasKnockback = (bool)dataRef[1];
            _enemyManager = (EnemyManager)dataRef[2];
            _attackName = (string)dataRef[3];

        }

        public override void Update() => State = State.SUCCESS;

        protected override void OnInit()
        {
            base.OnInit();
            Action<float> attack = (float str) => Tree.GetDatum<Transform>("target").GetComponent<Player.PlayerManager>().TakeHit(str + _damage, _hasKnockback, _enemyManager.transform.position);
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
            List<object> dataRef = Tree.GetData(_params);
            _enemyManager = (EnemyManager)dataRef[0];
            _attackName = (string)dataRef[1];
        }

        public override void Update() => State = State.SUCCESS;

        protected override void OnInit()
        {
            base.OnInit();
            _enemyManager.OnStrikePlayer -= Tree.GetDatum<Action<float>>(_attackName);
        }
    }
}
