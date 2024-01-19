using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public static MoveCamera Instance;
    [SerializeField] Transform cameraPosition;
    [SerializeField] Transform crouchCameraPosition;

    public Transform resetPosition;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        resetPosition = gameObject.transform;

        if (PlayerControllerScript.Instance.CurrentMovementState == PlayerControllerScript.PlayerMovementState.Crouching)
        {
            gameObject.transform.position = crouchCameraPosition.position;
        }
        else
        {
            gameObject.transform.position = cameraPosition.position;
        }
    }
}
