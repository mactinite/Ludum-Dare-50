using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    public string CollectableName = "First";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt(CollectableName, 1);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(CollectableName) == 1)
        {
            gameObject.SetActive(false);
        }
    }
}
