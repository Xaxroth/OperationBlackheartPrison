using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicUIObject : MonoBehaviour
{
    public GameObject ObjectToTurn;
    private Vector3 lastMousePosition;
    public bool shouldRotate;
    private float sensitivity = 30.0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && shouldRotate)
        {
            float deltaX = lastMousePosition.x - Input.mousePosition.x;

            float rotationAmount = deltaX * sensitivity * Time.deltaTime;

            ObjectToTurn.transform.Rotate(Vector3.up, rotationAmount, Space.World);
        }

        lastMousePosition = Input.mousePosition;
    }
}
