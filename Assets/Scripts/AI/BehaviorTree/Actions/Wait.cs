using UnityEngine;

namespace BehaviorTree.Actions
{
    public class Wait : CfAction
    {
        float _duration;
        float _timer = 0;

        public Wait(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _duration = Tree.GetDatum<float>(_params[0]);
        }

        public override void Update()
        {
            if (_timer >= _duration)
                State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _timer = 0;
        }
    }
}
