using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DigitalClock : MonoBehaviour
{
    public TMP_Text clockText;

    [Header("Clock Options")]
    public bool useSystemTime = true; // Toggle between system time or custom time
    public int startHour = 8;         // Custom start hour (0-23)
    public int startMinute = 15;      // Custom start minute (0-59)

    private DateTime customTime;

    void Start()
    {
        if (!useSystemTime)
        {
            // Initialize custom time
            customTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                startHour,
                startMinute,
                0
            );
        }

        StartCoroutine(UpdateClock());
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            DateTime now;

            if (useSystemTime)
            {
                now = DateTime.Now;
            }
            else
            {
                now = customTime;
                customTime = customTime.AddSeconds(1); // Increment by 1 second
            }

            clockText.text = now.ToString("HH:mm:ss");
            yield return new WaitForSeconds(1f);
        }
    }
}