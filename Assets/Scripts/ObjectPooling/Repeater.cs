using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Repeater : MonoBehaviour
{
    public UnityEvent OnRepeat = new UnityEvent();
    public float repeatDelay = 2f;

    private float timer = 0;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > repeatDelay)
        {
            OnRepeat.Invoke();
            timer = 0;
        }
    }
}
