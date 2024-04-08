using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public static CameraControllerScript Instance;

    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    [SerializeField] public Transform playerCamera;
    [SerializeField] public Transform Orientation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    public float xRotation;
    public float yRotation;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sensitivityX = 100;
        sensitivityY = 100;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (PlayerControllerScript.Instance.paralyzed) return;

        MouseInputChecker();
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        if (PlayerControllerScript.Instance.crouching)
        {
            playerCamera.transform.position = PlayerControllerScript.Instance._crouchFollowObject.transform.position;
        }
        else
        {
            playerCamera.transform.position = PlayerControllerScript.Instance._cameraFollowObject.transform.position;
        }
    }

    public void ResetCamera(Transform transform)
    {
        xRotation = 0;
        yRotation = transform.rotation.y;
        playerCamera.transform.localRotation = Quaternion.Euler(0, transform.rotation.y, 0);
        Orientation.transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }

    private void MouseInputChecker()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y"); 

        yRotation += mouseX * sensitivityX * multiplier;
        xRotation -= mouseY * sensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
