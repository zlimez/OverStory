using System.Collections.Generic;
using System;

using State = System.Int32;
using Event = System.Int32;

namespace AI.FSM
{
    public class Transition
    {
        public State StartState { get; }
        public Event Trigger { get; }
        public Func<object, bool> Cond { get; }
        public State NextState { get; }
        public Action<object> Action { get; }

        public Transition(State startState, Event trigger, State nextState, Action<object> action = null, Func<object, bool> cond = null)
        {
            StartState = startState;
            Trigger = trigger;
            NextState = nextState;
            Action = action;
            Cond = cond;
        }
    }

    public class FSM
    {
        public State CurrState { get; private set; }
        private readonly Dictionary<(State, Event), Transition> _transitions = new();
        private readonly Dictionary<State, Action<object>> _enterActions = new();
        private readonly Dictionary<State, Action<object>> _inActions = new();
        private readonly Dictionary<State, Action<object>> _exitActions = new();

        public FSM(State initState) => CurrState = initState;
        public void AddTransition(Transition transition, bool bidir = false)
        {
            _transitions[(transition.StartState, transition.Trigger)] = transition;
            if (bidir) _transitions[(transition.NextState, transition.Trigger)] = new Transition(transition.NextState, transition.Trigger, transition.StartState, transition.Action);
        }
        public void AddEntryAction(State state, Action<object> action) => _enterActions[state] = action;
        public void AddExitAction(State state, Action<object> action) => _exitActions[state] = action;
        public void AddInAction(State state, Action<object> action) => _inActions[state] = action;

        public void ProcessEvent(Event trigger, object input = null)
        {
            if (_transitions.TryGetValue((CurrState, trigger), out var transition))
            {
                if (transition.Cond != null && !transition.Cond(input)) return;
                bool stateChange = CurrState != transition.NextState;
                if (stateChange && _exitActions.TryGetValue(CurrState, out var exitAction))
                    exitAction?.Invoke(input);
                transition.Action?.Invoke(input);
                CurrState = transition.NextState;
                if (stateChange && _enterActions.TryGetValue(CurrState, out var enterAction))
                    enterAction?.Invoke(input);
            }
        }

        public void Tick<T>(T arg = default) { if (_inActions.TryGetValue(CurrState, out var action)) action?.Invoke(arg); }
    }
}