using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public GlobalControl gcRef;
    public Text timerText;

    private float timer;

    // public void Awake()
    // {
    //     timer = gcRef.timer;
    // }

    // Start is called before the first frame update
    public void Start()
    {
        if (GetComponent<Text>() != null)
            timerText = GetComponent<Text>();

        if (gcRef == null)
            gcRef = FindObjectOfType<GlobalControl>();
        timer = gcRef.timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            UpdateTimer();
        }

        else if (timer <= 0.0f)
        {
            timer = 0.0f;
            UpdateTimer();
            // GameOver();
        }
    }

    private void UpdateTimer()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = Mathf.Floor(timer % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;
    }
}
