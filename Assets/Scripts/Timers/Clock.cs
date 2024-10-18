using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace Abyss.TimeManagers
{
    [Serializable] public class ClockTickEvent : UnityEvent<int, int, int> { }

    public class Clock : MonoBehaviour
    {
        public int Hours;
        public int Minutes;
        public int Seconds;

        [System.NonSerialized] public ClockTickEvent OnClockTick = new();
        private IEnumerator _clockCoroutine;

        public void StartClock()
        {
            if (_clockCoroutine != null)
                StopCoroutine(_clockCoroutine);
            _clockCoroutine = ClockCoroutine();
            StartCoroutine(_clockCoroutine);
        }

        public void StopClock()
        {
            if (_clockCoroutine != null)
            {
                StopCoroutine(_clockCoroutine);
                _clockCoroutine = null;
            }
        }

        public void ResetClock()
        {
            if (_clockCoroutine != null)
            {
                StopCoroutine(_clockCoroutine);
                _clockCoroutine = null;
            }
            Hours = Minutes = Seconds = 0;
            OnClockTick.Invoke(Hours, Minutes, Seconds);
        }

        private IEnumerator ClockCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                IncrementTime();
                OnClockTick.Invoke(Hours, Minutes, Seconds);
            }
        }

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
                        Hours = 0;
                }
            }
        }
    }
}
