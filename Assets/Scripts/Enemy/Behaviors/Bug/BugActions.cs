using System.Collections.Generic;
using Abyss;
using Abyss.Environment.Enemy;
using Abyss.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorTree.Actions
{
    public class DropFrmCanopy : CfAction
    {
        Transform _transform, _leftEnd, _rightEnd;
        AnimationCurve _dropCurve;
        float _duration, _minSpace;
        string _jumpDestVarName, _dashDestVarName;
        SpriteManager _bugSprite;

        Vector3 _startPos, _dropPos;
        float _timer = 0;

        public DropFrmCanopy(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _leftEnd = Tree.GetDatum<Transform>(_params[0]);
            _rightEnd = Tree.GetDatum<Transform>(_params[1]);
            _transform = Tree.GetDatum<Transform>(_params[2]);
            _dropCurve = Tree.GetDatum<AnimationCurve>(_params[3]);
            _duration = Tree.GetDatum<float>(_params[4]);
            _minSpace = Tree.GetDatum<float>(_params[5]);
            _jumpDestVarName = _params[6];
            _dashDestVarName = _params[7];
            _bugSprite = Tree.GetDatum<SpriteManager>(_params[8]);
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

            if (player.GetComponent<PlayerController>().IsFacingLeft)
            {
                if (rspace >= _minSpace)
                    ChooseLeft(player, rspace);
                else ChooseRight(player, lspace);
            }
            else
            {
                if (lspace >= _minSpace)
                    ChooseRight(player, lspace);
                else ChooseLeft(player, rspace);
            }
            _startPos = new Vector3(_dropPos.x, _transform.position.y, _transform.position.z);
        }

        void ChooseLeft(Transform player, float rspace)
        {
            float rd = (float)new System.Random().NextDouble();
            _dropPos = new Vector3(player.position.x + _minSpace + rd * (rspace - _minSpace), _rightEnd.position.y, _rightEnd.position.z);
            Tree.SetDatum(_dashDestVarName, _leftEnd.position);
            Tree.SetDatum(_jumpDestVarName, new Vector3(_leftEnd.position.x, _transform.position.y, _transform.position.z));
            _bugSprite.FaceDir(Vector2.left);
        }

        void ChooseRight(Transform player, float lspace)
        {
            float rd = (float)new System.Random().NextDouble();
            _dropPos = new Vector3(player.position.x - _minSpace - rd * (lspace - _minSpace), _leftEnd.position.y, _leftEnd.position.z);
            Tree.SetDatum(_dashDestVarName, _rightEnd.position);
            Tree.SetDatum(_jumpDestVarName, new Vector3(_rightEnd.position.x, _transform.position.y, _transform.position.z));
            _bugSprite.FaceDir(Vector2.right);
        }
    }

    public class Dash : GotoTargetByCurve
    {
        static readonly int _obstacleLayerMask = 1 << (int)AbyssSettings.Layers.Obstacle;

        string _pitJumpDelayVarName;
        float _scanPitRaycastDist;
        EnemyManager _enemyManager;
        SpriteManager _bugSprite;

        readonly HashSet<int> _pitIds = new();

        public Dash(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _pitJumpDelayVarName = _params[6];
            _scanPitRaycastDist = Tree.GetDatum<float>(_params[7]);
            _enemyManager = Tree.GetDatum<EnemyManager>(_params[8]);
            _bugSprite = Tree.GetDatum<SpriteManager>(_params[9]);
        }

        public override void Update()
        {
            if (_timer < _duration)
            {
                _transform.position = Vector3.Lerp(_startPos, _destPos, _mvmtCurve.Evaluate(_timer / _duration));
                RaycastHit2D hit = Physics2D.Raycast(_transform.position, _bugSprite.forward, _scanPitRaycastDist, _obstacleLayerMask);
                if (hit.collider != null && hit.collider.CompareTag("Pit (Construct)") && !_pitIds.Contains(hit.collider.GetInstanceID()))
                {
                    _pitIds.Add(hit.collider.GetInstanceID());
                    var pit = hit.collider.GetComponent<Pit>();
                    pit.TakeDmg();
                    _enemyManager.TakeHit(pit.Damage);
                    Tree.SetDatum(_pitJumpDelayVarName, pit.JumpDelay);
                }
            }
            else State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _pitIds.Clear();
        }
    }
}
