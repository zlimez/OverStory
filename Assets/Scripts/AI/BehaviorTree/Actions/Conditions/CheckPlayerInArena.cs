namespace BehaviorTree.Actions
{
    public class CheckPlayerInArena : CfAction
    {
        Arena _arena;
        public CheckPlayerInArena(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            var dataRef = Tree.GetData(_params);
            _arena = (Arena)dataRef[0];
        }

        public override void Update()
        {
            State = _arena.PlayerIn ? State.SUCCESS : State.FAILURE;
            if (State == State.SUCCESS)
                Tree.SetDatum("target", _arena.Player.transform);
        }
    }
}
