using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleAfterTime : MonoBehaviour
{
    public ObjectPool poolReference;
    public float time;
    float timer;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > time)
        {
            poolReference.ReturnGameObject(gameObject);
        }
    }
}
