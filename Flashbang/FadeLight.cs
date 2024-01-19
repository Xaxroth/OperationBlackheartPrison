using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLight : MonoBehaviour
{
    [SerializeField] private Light ProjectilePointLight;
    private float fadeDuration = 1.0f;
    public void Start()
    {
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = ProjectilePointLight.intensity;

        while (elapsedTime < fadeDuration)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / fadeDuration);

            ProjectilePointLight.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ProjectilePointLight.intensity = 0f;
    }
}
