using UnityEngine;

namespace BehaviorTree.Actions
{
    public class XFaceTarget : CfAction
    {
        SpriteManager _spriteManager;

        public XFaceTarget(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _spriteManager = Tree.GetDatum<SpriteManager>(_params[0]);
        }

        public override void Update()
        {
            _spriteManager.Face(Tree.GetDatum<Transform>("target").position);
            State = State.SUCCESS;
        }
    }
}
