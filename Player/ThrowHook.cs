using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{
    public static ThrowHook Instance;

    public GameObject Hook;
    public GameObject HookWeapon;

    public Animator PlayerAnimator;

    public Transform HookTransform;

    public bool canThrow = true;
    public bool chargingThrow = false;

    public float power = 0;
    public float maxPower = 1;
    public float chargeRate = 0.5f;
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
    }

    private IEnumerator PlayAnimation()
    {
        PlayerAnimator.SetBool("LeftSwing", true);
        yield return new WaitForSeconds(0.1f);
        PlayerAnimator.SetBool("LeftSwing", false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canThrow)
        {
            chargingThrow = true;
        }

        if (Input.GetMouseButton(1) && chargingThrow)
        {
            if (power < maxPower)
            {
                power += chargeRate * Time.deltaTime;
            }
        }

        if (Input.GetMouseButtonUp(1) && chargingThrow)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.HookThrow, 1.0f);
            canThrow = false;
            chargingThrow = false;
            StartCoroutine(PlayAnimation());
            GameObject hook = Instantiate(Hook, HookTransform.position, PlayerControllerScript.Instance.CinemachineCamera.transform.rotation);
            hook.GetComponent<HookScript>().caster = transform;

            if (power < 0.5f)
            {
                hook.GetComponent<HookScript>().spearPower = 0.5f;
            }
            else
            {
                hook.GetComponent<HookScript>().spearPower = power;
            }

            power = 0;
        }
    }
}
