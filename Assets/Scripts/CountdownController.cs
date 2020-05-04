using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public TextMeshProUGUI textCountdown;
    public float leftSeconds;
    public bool started = false;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(StartTimer());
    }
    // Update is called once per frame
    void Update()
    {
        if(started) {
            leftSeconds = Mathf.Max(0, leftSeconds - Time.deltaTime);
        }

        int Minutes = (int)(leftSeconds / 60);
        int Seconds = (int)(leftSeconds % 60);
        int MillSeconds = (int)(((decimal)leftSeconds % 1) * 60);
        System.TimeSpan ts = new System.TimeSpan(0, Minutes, Seconds, MillSeconds);
        textCountdown.text = ts.ToString("c");
    }

    IEnumerator StartTimer() {
        yield return new WaitForSeconds(2);
        started = true;
    }
}
