using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using Utils;

namespace BehaviorTree.Actions
{
    public class DroneFollow : CfAction
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

        public DroneFollow(string[] parameters) : base(parameters) { }

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
                    _spriteManager.FaceDir(currVelocity);
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

    public class Hover : CfAction
    {
        string _targetName;
        float _hoverInterval;
        Vector3 _amp;
        Transform _transform;

        Transform _hoverCenter;
        float _timer = 0f;

        public Hover(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _targetName = _params[0];
            _amp = Tree.GetDatum<Vector3>(_params[1]);
            _hoverInterval = Tree.GetDatum<float>(_params[2]);
            _transform = Tree.GetDatum<Transform>(_params[3]);
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > _hoverInterval)
                _timer = 0f;
            _transform.position = Curves.SinLerpVector(_hoverCenter.position, _amp, _timer / _hoverInterval);
        }

        protected override void OnInit()
        {
            base.OnInit();
            _hoverCenter = Tree.GetDatum<Transform>(_targetName);
        }
    }

    public class DroneInDefault : CfAction
    {
        Light2D _droneLight;
        float _intensity;
        Color _color;
        Transform _transform;

        public DroneInDefault(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _droneLight = (Light2D)dataRef[0];
            _intensity = (float)dataRef[1];
            _color = (Color)dataRef[2];
            _transform = (Transform)dataRef[3];
        }

        public override void Update() => State = _transform.eulerAngles.z == 0 && _droneLight.intensity == _intensity && _droneLight.color == _color ? State.SUCCESS : State.FAILURE;
    }

    public class DroneAdjust : CfAction
    {
        Light2D _droneLight;
        float _adjustTime, _intensity, zRot, _tZRot;
        Color _color;
        AnimationCurve _adjustCurve;
        Transform _transform;
        SpriteManager _spriteManager;

        float _timer;

        public DroneAdjust(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _droneLight = (Light2D)dataRef[0];
            _adjustTime = (float)dataRef[1];
            _intensity = (float)dataRef[2];
            _color = (Color)dataRef[3];
            zRot = (float)dataRef[4];
            _adjustCurve = (AnimationCurve)dataRef[5];
            _transform = (Transform)dataRef[6];
            _spriteManager = (SpriteManager)dataRef[7];
        }

        public override void Update()
        {
            if (_timer > _adjustTime)
            {
                _droneLight.intensity = _intensity;
                _droneLight.color = _color;
                _transform.rotation = Quaternion.Euler(0, 0, zRot);
                State = State.SUCCESS;
            }
            else
            {
                _droneLight.intensity = _adjustCurve.Evaluate(_timer / _adjustTime) * _intensity;
                _droneLight.color = Color.Lerp(Color.clear, _color, _adjustCurve.Evaluate(_timer / _adjustTime));
                _transform.rotation = Quaternion.Euler(0, 0, _tZRot * _adjustCurve.Evaluate(_timer / _adjustTime));
                _timer += Time.deltaTime;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();
            _timer = 0f;
            _tZRot = zRot * _spriteManager.forward.x > 0 ? -1 : -1;
        }
    }
}
