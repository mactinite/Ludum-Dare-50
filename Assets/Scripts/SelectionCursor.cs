using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionCursor : MonoBehaviour
{
    public Image image;
    public float SmoothTime = 0.01f;
    public GameObject initialSelection;
    Vector2 startPos;
    RectTransform rectTransform;
    RectTransform targetRectTransform;
    GameObject selectedGameObject;
    public PlayerInput playerInput;

    private void Awake()
    {

        rectTransform = transform as RectTransform;
        startPos = image.rectTransform.position;
        image.gameObject.SetActive(false);
        
    }

    void OnEnable()
    {
        image.rectTransform.position = startPos;
        if (initialSelection)
        {
            EventSystem.current.SetSelectedGameObject(initialSelection);
        }
    }

    private void Update()
    {
        if (selectedGameObject != EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject != null)
        {
            image.gameObject.SetActive(true);
            selectedGameObject = EventSystem.current.currentSelectedGameObject;
            targetRectTransform = EventSystem.current.currentSelectedGameObject.transform as RectTransform;


            //rectTransform.pivot = targetRectTransform.pivot;
            //rectTransform.anchorMax = targetRectTransform.anchorMax;
            //rectTransform.anchorMin = targetRectTransform.anchorMin;

        }

        if(EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy || playerInput.currentControlScheme != "Gamepad" )
        {
            image.gameObject.SetActive(false);
        } else
        {
            image.gameObject.SetActive(true);
        }

        if (targetRectTransform)
        {
            image.rectTransform.position = targetRectTransform.position;
            image.rectTransform.sizeDelta = targetRectTransform.sizeDelta;
            image.rectTransform.pivot = targetRectTransform.pivot;
        }
    }
}
