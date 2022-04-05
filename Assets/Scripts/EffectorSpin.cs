using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectorSpin : MonoBehaviour
{
    public float spinSpeed = 10f;
    float angle = 0;
    public bool randomRotationAxis = false;
    public Vector3 rotateAround = Vector3.forward;
    public bool ignoreTimescale;
    private void Start()
    {
        if (randomRotationAxis)
        {
            rotateAround = new Vector3(Random.value, Random.value, Random.value);
        }
    }

    public void UpdateRandomDirection()
    {

        var x = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.time);
        var y = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.time);
        var z = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.time);

        if (ignoreTimescale)
        {
            x = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.unscaledTime);
            y = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.unscaledTime);
            z = RandomNoise.Get1DNoiseFloat((uint)Random.value * int.MaxValue, (uint)Time.unscaledTime);
        } 

        rotateAround = new Vector3(x, y, z);
    }

    // Update is called once per frame
    void Update()
    {
        if (ignoreTimescale)
        {
            angle = Time.unscaledDeltaTime * spinSpeed;
        } else
        {
            angle = Time.deltaTime * spinSpeed;
        }

        transform.Rotate(rotateAround, angle);

    }
}
