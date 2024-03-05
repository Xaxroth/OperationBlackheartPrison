using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIBoundary : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DynamicUIObject ObjectToCheck;
    public Button ButtonToBePressed;
    private bool isHovering = false;

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StopRotate();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StartRotate();
    }

    public void OnDrag(PointerEventData eventData)
    {
        StartRotate();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StopRotate();
    }

    public void StartRotate()
    {
        ObjectToCheck.shouldRotate = true;
    }

    public void StopRotate()
    {
        ObjectToCheck.shouldRotate = false;
    }
}
