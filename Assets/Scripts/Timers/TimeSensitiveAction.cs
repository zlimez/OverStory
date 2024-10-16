using System;

public class TimeSensitiveAction: IComparable<TimeSensitiveAction>
{
    // NOTE: Time refers to time left in countdown
    public float ScheduledTime { get; private set; }
    private Action action;

    public TimeSensitiveAction(float scheduledTime, Action action) {
        this.ScheduledTime= scheduledTime;
        this.action = action;
    }

    public void Execute() {
        action?.Invoke();
    }

    public int CompareTo(TimeSensitiveAction other) {
        // NOTE: Reverse execution order to be consistent with time left
        return -ScheduledTime.CompareTo(other.ScheduledTime);
    }
}
