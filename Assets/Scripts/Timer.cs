using System;
using TMPro;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float startTime;
    private bool isRunning = false;
    public bool IsRunning => isRunning;

    // StartTimer時に呼ばれるアクション
    public Action onStartTimer = null;

    void Update()
  {
    if (isRunning)
    {
      float elapsedTime = GetElapsedTime();
      int minutes = (int)(elapsedTime / 60);
      int seconds = (int)(elapsedTime % 60);
      int milliseconds = (int)((elapsedTime * 100) % 100);
      timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
  }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        onStartTimer?.Invoke();
    }

    public float GetElapsedTime()
    {
        if (isRunning)
        {
            return Time.time - startTime;
        }
        return 0f;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}