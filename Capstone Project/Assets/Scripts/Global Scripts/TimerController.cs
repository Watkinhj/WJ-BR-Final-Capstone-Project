using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    public TMP_Text timerText;

    private int hours = 9;
    private int minutes = 0;

    private float elapsedTime = 0f;
    public float interval = 2f; // Adjust this value for the speed of time passing

    private bool timerStopped = false;

    void Start()
    {
        // Set initial time to 9:00 AM
        hours = 9;
        minutes = 0;

        UpdateTimerText();
    }

    void Update()
    {
        if (!timerStopped)
        {
            // Update the timer every 'interval' seconds
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= interval)
            {
                elapsedTime = 0f;

                // Increase minutes
                minutes += 1; // Since minutes pass every 2 seconds
                if (minutes >= 60)
                {
                    minutes -= 60;
                    hours++;
                    if (hours >= 24)
                    {
                        hours = 0;
                    }
                }

                UpdateTimerText();

                // Check if it's 5:00 PM (17:00)
                if (hours == 17 && minutes == 0)
                {
                    timerStopped = true;
                    Debug.Log("Timer stopped at 5:00 PM.");
                }
            }
        }
    }

    void UpdateTimerText()
    {
        // Convert hours to 12-hour format
        int displayHours = hours > 12 ? hours - 12 : hours;
        // Determine AM/PM
        //string amPm = hours >= 12 ? "PM" : "AM";

        // Format the time string
        string timeString = string.Format("{0:D1}:{1:D2}", displayHours, minutes);
        //string timeString = string.Format("{0:D1}:{1:D2} {2}", displayHours, minutes, amPm);
        timerText.text = timeString;
    }
}
