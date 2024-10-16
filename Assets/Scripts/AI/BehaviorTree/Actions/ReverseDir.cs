using UnityEngine;

namespace BehaviorTree.Actions
{
    public class ReverseDir : CfAction
    {
        SpriteManager _spriteManager;

        public ReverseDir(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _spriteManager = Tree.GetDatum<SpriteManager>(_params[0]);
        }

        public override void Update()
        {
#if DEBUG || UNITY_EDITOR
            Debug.Log("Reversed");
#endif
            _spriteManager.Flip();
            State = State.SUCCESS;
        }
    }
}
