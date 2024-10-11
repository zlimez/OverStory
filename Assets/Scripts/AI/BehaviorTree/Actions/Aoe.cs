using System.Collections.Generic;
using Abyss.Environment.Enemy.Anim;
using BehaviorTree;
using UnityEngine;

namespace BehaviorTree.Actions
{
    public class Aoe : CfAction
    {
        ParticleSystem _particleSystem;
        BloomAnim _bloomAnim;
        float _duration;
        float _timer = 0;

        public Aoe(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _particleSystem = (ParticleSystem)dataRef[0];
            _bloomAnim = (BloomAnim)dataRef[1];
            _duration = (float)dataRef[2];
        }

        public override void Update()
        {
            if (_timer == 0)
            {
                _particleSystem.Play();
                _bloomAnim.TransitionToState(BloomAnim.State.Wiggle.ToString());
            }
            else if (_timer >= _duration)
            {
                _particleSystem.Stop();
                _bloomAnim.TransitionToState(BloomAnim.State.Idle.ToString());
                State = State.SUCCESS;
            }
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _timer = 0;
        }
    }
}
