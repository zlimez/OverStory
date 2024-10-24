using System;

namespace Abyss.TimeManagers
{
    public class TimeSensitiveAction : IComparable<TimeSensitiveAction>
    {
        // NOTE: Time refers to time left in countdown
        public float ScheduledTime { get; private set; }
        private readonly Action action;

        public TimeSensitiveAction(float scheduledTime, Action action)
        {
            this.ScheduledTime = scheduledTime;
            this.action = action;
        }

        public void Execute() => action?.Invoke();
        public int CompareTo(TimeSensitiveAction other) => -ScheduledTime.CompareTo(other.ScheduledTime);
    }
}
