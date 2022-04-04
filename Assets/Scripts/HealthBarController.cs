using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Image fillImage;

    public float dampingSpeed;
    private float _currentValue = 1;
    private float _velocity = 0;


    private void Update()
    {
        fillImage.fillAmount = Mathf.SmoothDamp(fillImage.fillAmount, _currentValue, ref _velocity, dampingSpeed);
    }

    public void SetValue(float val)
    {
        _currentValue = val;
    }

}
