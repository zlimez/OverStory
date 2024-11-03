namespace Abyss.Environment.Enemy.Anim
{
    public class BugAnim : PortraitAnim
    {
        public enum State { Idle, Dash, Preleap, Drop, Death }


        protected override void Awake()
        {
            base.Awake();
            currState = State.Idle.ToString();
        }

        protected override void OnEnable()
        {
            _playDefeatAnim = () => TransitionToState(State.Death.ToString());
            base.OnEnable();
        }
    }
}