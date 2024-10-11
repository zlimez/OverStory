using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

// TODO: Grid based A*
namespace BehaviorTree.Actions
{
    public class FollowTarget : CfAction
    {
        Transform _target, _transform;
        float _smoothTime;
        Transform _hoverTopTfm, _hoverBtmTfm;
        float _hoverInterval;

        float _dist2Stop, _dist2Start; // Once in this distance, object will rest
        float _dist2StopSq, _dist2StartSq;
        Vector3 hoverTop, hoverBtm;
        SpriteManager _spriteManager;
        Vector3 currVelocity;

        bool atRest = false;
        float _restTimer = 0f;

        public FollowTarget(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _target = (Transform)dataRef[0];
            _transform = (Transform)dataRef[1];
            _smoothTime = (float)dataRef[2];
            _hoverTopTfm = (Transform)dataRef[3];
            _hoverBtmTfm = (Transform)dataRef[4];
            _hoverInterval = (float)dataRef[5];
            _dist2Stop = (float)dataRef[6];
            _dist2Start = (float)dataRef[7];
            _spriteManager = (SpriteManager)dataRef[8];
            _dist2StopSq = _dist2Stop * _dist2Stop;
            _dist2StartSq = _dist2Start * _dist2Start;
            Assert.IsTrue(_dist2Start > _dist2Stop);
        }

        public override void Update()
        {
            if (!atRest)
            {
                if ((_target.position - _transform.position).sqrMagnitude > _dist2StopSq)
                {
                    _transform.position = Vector3.SmoothDamp(_transform.position, _target.position, ref currVelocity, _smoothTime);
                    _spriteManager.FaceMoveDir(currVelocity);
                }
                else
                {
                    atRest = true;
                    hoverTop = _hoverTopTfm.position;
                    hoverBtm = _hoverBtmTfm.position;
                    _restTimer = 0f;
                }
            }
            else
            {
                if ((_target.position - _transform.position).sqrMagnitude > _dist2StartSq)
                    atRest = false;
                else
                {
                    _restTimer += Time.deltaTime;
                    if (_restTimer > _hoverInterval)
                        _restTimer = 0f;
                    _transform.position = Curves.SinLerpVector((hoverTop + hoverBtm) / 2, (hoverTop - hoverBtm) / 2, _restTimer / _hoverInterval);
                }
            }
        }
    }
}
