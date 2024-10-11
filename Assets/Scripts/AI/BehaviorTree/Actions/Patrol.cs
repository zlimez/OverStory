using System.Collections.Generic;
using UnityEngine;
using Abyss.Environment.Enemy.Anim;

namespace BehaviorTree.Actions
{
    public class Patrol : CfAction
    {
        private int _currWaypoint = 0;
        private float _waitCounter = 0f;
        private bool _waiting = false;
        private float _speed;
        private Transform[] _waypoints;
        private Transform _transform;
        private PortraitAnim _chargeTypeAnim;
        private SpriteManager _spriteManager;
        private float _waitTime;
        string _idleAnim;
        string _walkAnim;

        public Patrol(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _speed = (float)dataRef[0];
            _waypoints = (Transform[])dataRef[1];
            _transform = (Transform)dataRef[2];
            _waitTime = (float)dataRef[3];
            _spriteManager = (SpriteManager)dataRef[4];
            _chargeTypeAnim = (PortraitAnim)dataRef[5];
            _idleAnim = (string)dataRef[6];
            _walkAnim = (string)dataRef[7];
        }

        public override void Update()
        {
            if (_waiting)
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                    _chargeTypeAnim.TransitionToState(_walkAnim);
                }
            }
            else
            {
                Transform wp = _waypoints[_currWaypoint];
                if (Vector2.Distance(_transform.position, wp.position) < 0.01f)
                {
                    _transform.position = wp.position;
                    _waitCounter = 0f;
                    _waiting = true;

                    _currWaypoint = (_currWaypoint + 1) % _waypoints.Length;
                    _chargeTypeAnim.TransitionToState(_idleAnim);
                }
                else
                {
                    _chargeTypeAnim.TransitionToState(_walkAnim);
                    _transform.position = Vector2.MoveTowards(_transform.position, new Vector3(wp.position.x, _transform.position.y, _transform.position.z), _speed * Time.deltaTime);
                    _spriteManager.Face(wp.position);
                }
            }
        }
    }
}
