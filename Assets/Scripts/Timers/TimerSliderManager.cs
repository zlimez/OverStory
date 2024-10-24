using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace Abyss.TimeManagers
{
    public class TimerSliderManager : MonoBehaviour
    {
        [SerializeField] private CountdownTimer timer;
        [SerializeField] private Slider slider;
        [SerializeField] protected GameObject timerSliderObject;
        [SerializeField] private PlayableDirector timeline;
        [SerializeField] private Image fillColor;
        private Vector3 startColor, endColor, currColor, gradient;

        protected virtual void OnEnable()
        {
            // Debug.Log("Timer Slider Manager Enable");
            // Subscribe to the timeLeftChangedEvent
            timer.OnTimerChange.AddListener(OnTimeLeftChanged);
            timer.OnTimerStart.AddListener(OnTimerStart);
            timer.OnTimerExpire.AddListener(OnTimerExpire);
        }

        protected virtual void OnDisable()
        {
            // Debug.Log("Timer Slider Manager Disable");
            // Unsubscribe from the timeLeftChangedEvent
            timer.OnTimerChange.RemoveListener(OnTimeLeftChanged);
            timer.OnTimerStart.RemoveListener(OnTimerStart);
            timer.OnTimerExpire.RemoveListener(OnTimerExpire);

        }

        private void OnTimerStart()
        {
            timerSliderObject.SetActive(true);
            timeline.Play();
            slider.maxValue = timer.TotalDuration;
            slider.value = timer.TimeLeft;
            InitialiseColor(timer.TotalDuration);
        }

        private void OnTimeLeftChanged(float newTimeLeft)
        {
            // Debug.Log("Timer Slider Manager Time Left Change");
            // Update the slider value
            if (newTimeLeft > 0.2f)
                slider.value = newTimeLeft;
            UpdateColor(newTimeLeft);
        }

        private void OnTimerExpire() => timerSliderObject.SetActive(false);

        private void InitialiseColor(float totalDuration)
        {
            startColor = new Vector3(141, 221, 255);
            endColor = new Vector3(255, 16, 11);
            gradient = endColor - startColor;
        }

        private void UpdateColor(float newTimeLeft)
        {
            currColor = startColor + gradient * (timer.TotalDuration - newTimeLeft) / timer.TotalDuration;
            byte R = (byte)currColor.x;
            byte G = (byte)currColor.y;
            byte B = (byte)currColor.z;
            fillColor.color = new Color32(R, G, B, 255);
        }
    }
}
