using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zekrael : MonoBehaviour
{
    public Transform headBone;
    private Quaternion initialRotation;

    void Start()
    {
        // Capture the initial rotation of the headBone
        initialRotation = headBone.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //HeadTurn();
    }

    public void HeadTurn()
    {
        Vector3 playerDirection = PlayerControllerScript.Instance.transform.position - transform.position;
        Vector3 forwardDirection = transform.forward;

        float dotProduct = Vector3.Dot(playerDirection.normalized, forwardDirection);

        if (dotProduct > 0)
        {
            Vector3 directionToPlayer = (PlayerControllerScript.Instance.CinemachineCamera.transform.position - headBone.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Calculate the rotation with the initial offset only if the head is turning
            Quaternion finalRotation = Quaternion.Slerp(headBone.rotation, targetRotation, 5 * Time.deltaTime);

            // Apply initial rotation offset
            finalRotation *= initialRotation;

            headBone.rotation = finalRotation;
        }
    }
}
