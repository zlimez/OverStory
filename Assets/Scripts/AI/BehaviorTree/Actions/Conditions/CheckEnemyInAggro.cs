namespace BehaviorTree.Actions
{
    public class CheckPlayerInAggro : CfAction
    {
        Aggro _aggro;
        public CheckPlayerInAggro(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            var dataRef = Tree.GetData(_params);
            _aggro = (Aggro)dataRef[0];
        }

        public override void Update()
        {
            State = _aggro.PlayerIn ? State.SUCCESS : State.FAILURE;
            if (State == State.SUCCESS)
                Tree.SetDatum("target", _aggro.Player.transform);
        }
    }
}