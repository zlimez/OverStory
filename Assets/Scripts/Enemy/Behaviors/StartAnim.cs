using Abyss.Environment.Enemy.Anim;
using BehaviorTree;

namespace Abyss.Environment.Enemy
{
    public class StartAnim : CfAction
    {
        PortraitAnim _portraitAnim;
        string _animState;

        public StartAnim(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _portraitAnim = Tree.GetDatum<PortraitAnim>(_params[0]);
            _animState = _params[1];
        }

        public override void Update() => State = State.SUCCESS;

        protected override void OnInit()
        {
            base.OnInit();
            _portraitAnim.TransitionToState(_animState);
        }
    }
}
