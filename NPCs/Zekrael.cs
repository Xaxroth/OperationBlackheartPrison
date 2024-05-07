using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zekrael : MonoBehaviour
{
    public Transform headBone;
    private Quaternion initialRotation;

    public static Zekrael Instance;
    public GameObject TeleportationEffect;

    public static bool Disappeared;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (Disappeared)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
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

    public void TeleportAway()
    {
        StartCoroutine(Teleport());
    }

    public IEnumerator Teleport()
    {
        Animator enemyAnimator = gameObject.GetComponent<Animator>();
        enemyAnimator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.75f);
        GameObject TeleportSpell = Instantiate(TeleportationEffect, transform.position, Quaternion.identity);
        Disappeared = true;
        gameObject.SetActive(false);
    }
}
