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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canThrow)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.HookThrow, 1.0f);
            canThrow = false;
            StartCoroutine(PlayAnimation());
            GameObject hook = Instantiate(Hook, HookTransform.position, PlayerControllerScript.Instance.CinemachineCamera.transform.rotation);
            hook.GetComponent<HookScript>().caster = transform;
        }
    }
}
