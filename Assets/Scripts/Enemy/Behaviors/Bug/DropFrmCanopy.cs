using System.Collections.Generic;
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
            List<object> dataRef = Tree.GetData(_params);
            _leftEnd = (Transform)dataRef[0];
            _rightEnd = (Transform)dataRef[1];
            _transform = (Transform)dataRef[2];
            _dropCurve = (AnimationCurve)dataRef[3];
            _duration = (float)dataRef[4];
            _minSpace = (float)dataRef[5];
            _jumpDestVarName = (string)dataRef[6];
            _dashDestVarName = (string)dataRef[7];
            _bugSprite = (SpriteManager)dataRef[8];
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
}
