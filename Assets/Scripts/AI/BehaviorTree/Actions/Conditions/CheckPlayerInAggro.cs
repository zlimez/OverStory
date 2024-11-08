namespace BehaviorTree.Actions
{
    public class CheckPlayerInAggro : CfAction
    {
        Aggro _aggro;
        string _targetName;
        public CheckPlayerInAggro(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _aggro = Tree.GetDatum<Aggro>(_params[0]);
            _targetName = _params[1];
        }

        public override void Update()
        {
            State = _aggro.PlayerIn ? State.SUCCESS : State.FAILURE;
            if (State == State.SUCCESS)
                Tree.SetDatum(_targetName, _aggro.Player.transform);
        }
    }
}