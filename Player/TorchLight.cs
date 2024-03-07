using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 2f; // Increase the speed for more noticeable flickering

    public Light torchLight;
    private float baseIntensity;
    private float targetIntensity;

    void OnEnable()
    {
        baseIntensity = torchLight.intensity;
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity) * baseIntensity;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f) / flickerSpeed);
        }
    }

    void Update()
    {
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);
    }
}
