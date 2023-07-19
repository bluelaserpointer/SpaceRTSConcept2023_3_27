using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Timer : MonoBehaviour
{
    public float time;
    public UnityEvent timerEvent;
    public float PassedTime => Time.timeSinceLevelLoad - startTime;
    float startTime;
    public bool Invocked { get; private set; }
    private void Start()
    {
        startTime = Time.timeSinceLevelLoad;
    }
    private void Update()
    {
        if (!Invocked && PassedTime > time)
        {
            Invocked = true;
            timerEvent.Invoke();
        }
    }
}
