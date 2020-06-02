using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using SunnyCat.Tools;

public class CountdownController : Singleton<CountdownController>
{
    [Serializable]
    public class Round
    {
        public Color color;
        public float leftSeconds;
    }

    public Round[] rounds;
    public TextMeshProUGUI textCountdown;
    public float startDelaySeconds = 2.0f;
    public UnityEvent OnRoundEnded;
    private bool started = false;
    private bool running = false;
    private float currentLeftSeconds;
    public int CurrentRount {get;private set;}

    public Color CurrentColor
    {
        get
        {
            return rounds[CurrentRount].color;
        }
    }
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(StartTimer());
    }
    // Update is called once per frame
    void Update()
    {
        if(!running)
        {
            return;
        }
        if(started)
        {

            currentLeftSeconds = currentLeftSeconds - Time.deltaTime;
            if(currentLeftSeconds < 0) {
                currentLeftSeconds = 0;
                Pause();
                OnRoundEnded.Invoke();
            }
        }

        int Minutes = (int)(currentLeftSeconds / 60);
        int Seconds = (int)(currentLeftSeconds % 60);
        int MillSeconds = (int)(((decimal)currentLeftSeconds % 1) * 60);
        System.TimeSpan ts = new System.TimeSpan(0, Minutes, Seconds, MillSeconds);
        textCountdown.text = ts.ToString("c");
        textCountdown.color = CurrentColor;
    }

    IEnumerator StartTimer() {
        yield return new WaitForSeconds(startDelaySeconds);
        Restart();
    }
    
    public void Continue()
    {
        running = true;
    }
    public void Pause()
    {
        running = false;
    }
    public void Stop()
    {
        started = false;
    }
    public bool HasNextRound()
    {
        return CurrentRount < rounds.Length - 1;
    }
    public void NextRound()
    {
        CurrentRount++;
        if(CurrentRount >= rounds.Length)
        {
            Stop();
            currentLeftSeconds = 0;
            CurrentRount = rounds.Length - 1;
        }
        else
        {
            currentLeftSeconds = rounds[CurrentRount].leftSeconds;
            Continue();
        }
    }
    public void Restart()
    {
        CurrentRount = 0;
        started = true;
        currentLeftSeconds = rounds[CurrentRount].leftSeconds;
        Continue();
    }

}
