using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpUi : MonoBehaviour
{
    private bool isSpeedUp;
    private float speedUp = 3f;
    [SerializeField] GameObject showTimeline, hideTimeline;

    void Start() 
    {
        showTimeline.SetActive(false);
        hideTimeline.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("SpeedUp"))
        {
            isSpeedUp = !isSpeedUp;

            // Prevents playing the animation when the 
            // button is not pressed  
            if (!isSpeedUp && Time.timeScale != 1f)
            {
                Hide();
                Time.timeScale = 1f;
                return;
            }
        }

        // Speed is reset to normal after scene transition
        // Set it to speed-up time if in the previous scene
        // speed-up button is pressed
        if (isSpeedUp && Time.timeScale == 1f)
        {
            Show();
            Time.timeScale = speedUp;
            return;
        }
    }

    void Show() 
    {
        hideTimeline.SetActive(false);
        showTimeline.SetActive(true);
    }

    void Hide() 
    {
        hideTimeline.SetActive(true);
        showTimeline.SetActive(false);
    }
}
