using System;
using System.Collections.Generic;
using Abyss.Environment.Enemy;
using Abyss.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorTree.Actions
{
    public class DropFrmCanopy : CfAction
    {
        Transform _transform;
        Transform _leftEnd;
        Transform _rightEnd;
        AnimationCurve _dropCurve;
        float _duration;
        float _minSpace;
        string _jumpDestVarName;
        SpriteManager _bugSprite;

        Vector3 _startPos;
        Vector3 _dropPos;
        float _timer = 0;

        public DropFrmCanopy(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _leftEnd = (Transform)dataRef[0];
            _rightEnd = (Transform)dataRef[1];
            _transform = (Transform)dataRef[2];
            _dropCurve = (AnimationCurve)dataRef[3];
            _duration = (float)dataRef[4];
            _minSpace = (float)dataRef[5];
            _jumpDestVarName = (string)dataRef[6];
            _bugSprite = (SpriteManager)dataRef[7];
        }

        public override void Update()
        {
            if (_timer < _duration)
                _transform.position = Vector3.Lerp(_startPos, _dropPos, _dropCurve.Evaluate(_timer / _duration));
            else State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _timer = 0;
            var player = Tree.GetDatum<Transform>("target");
            float rspace = _rightEnd.position.x - player.position.x;
            float lspace = player.position.x - _leftEnd.position.x;
            Assert.IsTrue(rspace + lspace >= _minSpace, "The space bet left and right end point should be geq to min space");
            float rd = (float)new System.Random().NextDouble();
            if (player.GetComponent<PlayerController>().IsFacingLeft)
            {
                if (rspace >= _minSpace)
                {
                    _dropPos = new Vector3(player.position.x + _minSpace + rd * (rspace - _minSpace), _rightEnd.position.y, _rightEnd.position.z);
                    Tree.SetDatum(_jumpDestVarName, new Vector3(_leftEnd.position.x, _transform.position.y, _transform.position.z));
                    _bugSprite.FaceDir(Vector2.left);
                }
                else
                {
                    _dropPos = new Vector3(player.position.x - _minSpace - rd * (lspace - _minSpace), _leftEnd.position.y, _leftEnd.position.z);
                    Tree.SetDatum(_jumpDestVarName, new Vector3(_rightEnd.position.x, _transform.position.y, _transform.position.z));
                    _bugSprite.FaceDir(Vector2.right);
                }
            }
            else
            {
                if (lspace >= _minSpace)
                {
                    _dropPos = new Vector3(player.position.x - _minSpace - rd * (lspace - _minSpace), _leftEnd.position.y, _leftEnd.position.z);
                    Tree.SetDatum(_jumpDestVarName, new Vector3(_rightEnd.position.x, _transform.position.y, _transform.position.z));
                    _bugSprite.FaceDir(Vector2.right);
                }
                else
                {
                    _dropPos = new Vector3(player.position.x + _minSpace + rd * (rspace - _minSpace), _rightEnd.position.y, _rightEnd.position.z);
                    Tree.SetDatum(_jumpDestVarName, new Vector3(_leftEnd.position.x, _transform.position.y, _transform.position.z));
                    _bugSprite.FaceDir(Vector2.left);
                }
            }
            _startPos = new Vector3(_dropPos.x, _transform.position.y, _transform.position.z);
        }
    }

    public class DashLeftOrRight : CfAction
    {
        Transform _leftEnd;
        Transform _rightEnd;
        Transform _transform;
        AnimationCurve _mvmtCurve;
        EnemyManager _enemyManager;
        float _speed;
        float _damage;

        float _duration;
        Vector3 _startPos;
        float _timer = 0;
        Vector3 _destPos;

        public DashLeftOrRight(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _leftEnd = (Transform)dataRef[0];
            _rightEnd = (Transform)dataRef[1];
            _transform = (Transform)dataRef[2];
            _mvmtCurve = (AnimationCurve)dataRef[3];
            _speed = (float)dataRef[4];
            _damage = (float)dataRef[5];
            _enemyManager = (EnemyManager)dataRef[6];
        }

        public override void Update()
        {
            if (_timer == 0) _enemyManager.OnStrikePlayer += DashHit;
            if (_timer < _duration)
                _transform.position = Vector3.Lerp(_startPos, _destPos, _mvmtCurve.Evaluate(_timer / _duration));
            else
            {
                State = State.SUCCESS;
                _enemyManager.OnStrikePlayer -= DashHit;
            }
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _timer = 0;
            _startPos = _transform.position;
            _destPos = Tree.GetDatum<Transform>("target").position.x < _transform.position.x ? _leftEnd.position : _rightEnd.position;
            _duration = Math.Abs(_destPos.x - _startPos.x) / _speed;
        }

        void DashHit(float str)
        {
            Tree.GetDatum<Transform>("target").gameObject.GetComponent<PlayerManager>().TakeHit(str + _damage, true, _enemyManager.transform.position);
        }
    }
}
