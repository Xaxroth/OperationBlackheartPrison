using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicScript : MonoBehaviour
{
    public GameObject ShadowFlamePrefab;
    public GameObject DarkboltPrefab;
    public GameObject EldritchBlastPrefab;
    public Transform ShootPosition;
    public Transform launchPoint;
    public float maxChargeTime = 3.0f;
    public float maxLaunchForce = 10f;

    private float staminaDecrementAccumulator;

    private float currentChargeTime = 0.0f;
    private bool isCharging = false;

    public enum MagicType
    {
        Shadowflame,
        Darkbolt,
        EldritchBlast
    }

    public MagicType type;

    void Update()
    {
        // Start charging when mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentChargeTime = 0.0f;
        }

        // While the mouse button is held down, increase the charge time
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            float decrementAmount = 100 * Time.deltaTime;
            staminaDecrementAccumulator += decrementAmount;

            // Apply the accumulated decrement to playerStamina as an integer
            int staminaDecrementInt = Mathf.FloorToInt(staminaDecrementAccumulator);
            if (staminaDecrementInt > 0)
            {
                PlayerControllerScript.Instance.playerStamina -= staminaDecrementInt;
                staminaDecrementAccumulator -= staminaDecrementInt;
            }

            // Ensure currentChargeTime does not exceed maxChargeTime
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0, maxChargeTime);

            if (currentChargeTime >= maxChargeTime)
            {
                type = MagicType.EldritchBlast;
                isCharging = false;
                LaunchProjectile();
            }
        }

        // Launch the projectile when the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            if (isCharging)
            {
                if (currentChargeTime <= 0.33)
                {
                    type = MagicType.Shadowflame;
                    Debug.Log("Shadowflame");
                }
                else if (currentChargeTime > 0.33 && currentChargeTime < 0.66)
                {
                    type = MagicType.Darkbolt;
                    Debug.Log("Darkbolt");
                }
                else if (currentChargeTime >= 0.66)
                {
                    type = MagicType.EldritchBlast;
                    Debug.Log("Eldritch Blast");
                }
                LaunchProjectile();
                isCharging = false;
            }
        }
    }

    void LaunchProjectile()
    {
        switch (type)
        {
            case MagicType.Shadowflame:
                GameObject ShadowFlame = Instantiate(ShadowFlamePrefab, ShootPosition.position + new Vector3(0, 3, 0), ShootPosition.rotation);

                Rigidbody rb = ShadowFlame.GetComponent<Rigidbody>();

                float launchForce = Mathf.Lerp(0, maxLaunchForce, currentChargeTime / maxChargeTime);

                ShadowFlame.GetComponent<ProjectileScript>().force = 1;

                currentChargeTime = 0.0f;

                Destroy(ShadowFlame, 3.0f);
                break;
            case MagicType.Darkbolt:
                GameObject Darkbolt = Instantiate(DarkboltPrefab, ShootPosition.position + new Vector3(0, 3, 0), ShootPosition.rotation);

                Rigidbody DarkboltRb = DarkboltPrefab.GetComponent<Rigidbody>();

                float DarkboltForce = Mathf.Lerp(0, maxLaunchForce, currentChargeTime / maxChargeTime);

                Darkbolt.GetComponent<ProjectileScript>().force = 1;

                currentChargeTime = 0.0f;

                Destroy(Darkbolt, 3.0f);
                break;
            case MagicType.EldritchBlast:
                GameObject EldritchBlast = Instantiate(EldritchBlastPrefab, ShootPosition.position + new Vector3(0, 3, 0), ShootPosition.rotation);

                Rigidbody EldritchBlastRb = EldritchBlastPrefab.GetComponent<Rigidbody>();

                float EldritchBlastForce = Mathf.Lerp(0, maxLaunchForce, currentChargeTime / maxChargeTime);

                //EldritchBlast.GetComponent<ProjectileScript>().force = 1;

                currentChargeTime = 0.0f;

                Destroy(EldritchBlast, 3.0f);
                break;
        }
    }
}
