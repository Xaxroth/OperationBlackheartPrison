using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event1Trigger : MonoBehaviour
{
    bool eventActive = false;
    bool eventFinished = false;
    bool playerLooking = false;

    public float detectionRange = 10f;
    public GameObject Natia;

    public static bool EventFinished;

    private void Start()
    {
        if (!EventFinished)
        {
            DialogueManagerScript.Instance.Event1();
        }

        EventFinished = true;
    }

    private void Update()
    {
        if (eventActive)
        {
            CheckPlayerLooking();
        }
    }

    private void StartEvent()
    {
        float distanceToNatia = Vector3.Distance(PlayerControllerScript.Instance.CinemachineCamera.transform.position, Natia.transform.position);

        eventActive = true;

        if (Natia != null && eventActive)
        {
            Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerControllerScript.Instance.CinemachineCamera);

            if (!GeometryUtility.TestPlanesAABB(cameraPlanes, Natia.GetComponentInChildren<Renderer>().bounds))
            {
                Natia.SetActive(true);
            }
        }
    }

    private void CheckPlayerLooking()
    {
        float distanceToNatia = Vector3.Distance(PlayerControllerScript.Instance.CinemachineCamera.transform.position, Natia.transform.position);

        if (Natia != null && eventActive)
        {
            Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerControllerScript.Instance.CinemachineCamera);

            if (GeometryUtility.TestPlanesAABB(cameraPlanes, Natia.GetComponentInChildren<Renderer>().bounds))
            {
                playerLooking = true;
                HandlePlayerLooking();
                eventActive = false;
            }
        }
    }

    private void HandlePlayerLooking()
    {
        if (Natia.activeInHierarchy)
        {
            eventFinished = true;
            DialogueManagerScript.Instance.Event1();
        }
    }
}
