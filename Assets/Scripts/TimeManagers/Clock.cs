using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace Chronellium.TimeManagers
{
    /// <summary>
    /// A custom UnityEvent that takes hours, minutes, and seconds as arguments.
    /// </summary>
    [Serializable]
    public class ClockTickEvent : UnityEvent<int, int, int> { }

    /// <summary>
    /// A MonoBehaviour that simulates a clock and keeps track of the current hour, minute, and second.
    /// </summary>
    public class Clock : MonoBehaviour
    {
        /// <summary>
        /// The current hour.
        /// </summary>
        public int Hours;

        /// <summary>
        /// The current minute.
        /// </summary>
        public int Minutes;

        /// <summary>
        /// The current second.
        /// </summary>
        public int Seconds;

        /// <summary>
        /// The event triggered on each clock tick, passing the current hour, minute, and second as arguments.
        /// </summary>
        [System.NonSerialized] public ClockTickEvent OnClockTick = new ClockTickEvent();

        private IEnumerator _clockCoroutine;

        /// <summary>
        /// Starts the clock ticking.
        /// </summary>
        public void StartClock()
        {
            if (_clockCoroutine != null)
            {
                StopCoroutine(_clockCoroutine);
            }
            _clockCoroutine = ClockCoroutine();
            StartCoroutine(_clockCoroutine);
        }

        /// <summary>
        /// Stops the clock ticking.
        /// </summary>
        public void StopClock()
        {
            if (_clockCoroutine != null)
            {
                StopCoroutine(_clockCoroutine);
                _clockCoroutine = null;
            }
        }

        /// <summary>
        /// Resets the clock to 00:00:00.
        /// </summary>
        public void ResetClock()
        {
            if (_clockCoroutine != null)
            {
                StopCoroutine(_clockCoroutine);
                _clockCoroutine = null;
            }
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            OnClockTick.Invoke(Hours, Minutes, Seconds);
        }

        /// <summary>
        /// A coroutine that simulates the clock ticking every second.
        /// </summary>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private IEnumerator ClockCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                IncrementTime();
                OnClockTick.Invoke(Hours, Minutes, Seconds);
            }
        }

        /// <summary>
        /// Increments the time by one second, updating the hours, minutes, and seconds accordingly.
        /// </summary>
        private void IncrementTime()
        {
            Seconds++;
            if (Seconds >= 60)
            {
                Seconds = 0;
                Minutes++;
                if (Minutes >= 60)
                {
                    Minutes = 0;
                    Hours++;
                    if (Hours >= 24)
                    {
                        Hours = 0;
                    }
                }
            }
        }
    }
}
