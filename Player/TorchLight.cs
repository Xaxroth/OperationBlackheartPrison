using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 2f;

    public Light torchLight;
    private float baseIntensity;
    private float targetIntensity;
    private bool torchLit = false;

    public Animator PlayerArms;

    void OnEnable()
    {
        StartCoroutine(TorchLightUp());
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        PlayerControllerScript.Instance.switchingWeapon = true;
        PlayerArms.SetBool("PullOut", true);
        yield return new WaitForSeconds(0.7f);
        PlayerControllerScript.Instance.switchingWeapon = false;
        PlayerArms.SetBool("PullOut", false);
    }

    IEnumerator TorchLightUp()
    {
        torchLight.intensity = 0;
        float elapsedTime = 0f;
        float duration = 1.5f;

        while (torchLight.intensity < 40f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            torchLight.intensity = Mathf.Lerp(0f, maxIntensity, t);
            yield return null;
        }

        torchLight.intensity = 40f;
        yield return new WaitForSeconds(1f);
        torchLit = true;
    }

    //IEnumerator Flicker()
    //{
    //    //while (true)
    //    //{
    //    //    targetIntensity = Random.Range(minIntensity, maxIntensity) * baseIntensity;
    //    //    yield return new WaitForSeconds(Random.Range(0.05f, 0.2f) / flickerSpeed);
    //    //}
    //}

    void Update()
    {
        if (torchLit)
        {
            //torchLight.intensity = Mathf.Lerp(torchLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);
        }
    }
}
