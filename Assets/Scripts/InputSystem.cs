using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameObject cube;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        Debug.Log("up");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(Input.GetTouch(eventData.pointerId).position);

    }
}
