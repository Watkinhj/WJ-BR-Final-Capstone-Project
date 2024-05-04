using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    public TMP_Text timerText;
    public int hours = 9;
    public int minutes = 0;
    public float elapsedTime = 0f;
    public float interval = 2f;
    private bool timerStopped = false;
    public PlayerStats playerStats;
    public bool isFivePM = false;
    public EnemySpawner enemySpawner;

    public delegate void HourlyUpdateHandler(int hour);
    public event HourlyUpdateHandler OnHourChanged;

    void Start()
    {
        UpdateTimerText();
    }

    void Update()
    {
        if (!timerStopped)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= interval)
            {
                elapsedTime = 0f;
                minutes += 1;
                if (minutes >= 60)
                {
                    minutes = 0;
                    int oldHour = hours;
                    hours++;
                    if (hours >= 24)
                    {
                        hours = 0;
                    }

                    if (oldHour != hours) // Checks if hour has changed
                    {
                        OnHourChanged?.Invoke(hours);
                    }
                }

                UpdateTimerText();

                if (hours == 17 && minutes == 0)
                {
                    GameState.SetAfterFivePM();
                    timerStopped = true;
                    playerStats.is5PM = true;
                    isFivePM = true;
                    playerStats.TimeCardCheck();
                    Debug.Log("Timer stopped at 5:00 PM.");
                }
            }
        }
    }

    void UpdateTimerText()
    {
        int displayHours = hours > 12 ? hours - 12 : hours;
        string timeString = string.Format("{0:D1}:{1:D2}", displayHours, minutes);
        timerText.text = timeString;
    }
}
