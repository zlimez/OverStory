namespace BehaviorTree.Actions
{
    public class CheckPlayerInArena : CfAction
    {
        Arena _arena;
        string _targetName;

        public CheckPlayerInArena(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _arena = Tree.GetDatum<Arena>(_params[0]);
            _targetName = _params[1];
        }

        public override void Update()
        {
            State = _arena.PlayerIn ? State.SUCCESS : State.FAILURE;
            if (State == State.SUCCESS)
                Tree.SetDatum(_targetName, _arena.Player.transform);
        }
    }
}
