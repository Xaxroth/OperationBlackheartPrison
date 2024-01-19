using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider staminaSlider;
    public Material HealthMaterial;
    public Material EnergyMaterial;
    public float fillValue;
    public float fillMaxValue;

    void Start()
    {
        healthSlider = GetComponent<Slider>();
        fillValue = HealthMaterial.GetFloat("_FillLevel");

        if (HealthMaterial != null)
        {
            HealthMaterial.SetFloat("_FillLevel", 1.0f);
        }

        if (EnergyMaterial != null)
        {
            EnergyMaterial.SetFloat("_FillLevel", 1.0f);
        }
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        float fillPercentage = healthSlider.value / healthSlider.maxValue;

        HealthMaterial.SetFloat("_FillLevel", fillPercentage);
    }

    public void SetMaxStamina(int health)
    {
        staminaSlider.maxValue = health;
        staminaSlider.value = health;

        float fillPercentage = staminaSlider.value / staminaSlider.maxValue;

        EnergyMaterial.SetFloat("_FillLevel", fillPercentage);
    }

    public void SetHealth(int health)
    {
        healthSlider.value = (float)health;

        float fillPercentage = healthSlider.value / healthSlider.maxValue;

        HealthMaterial.SetFloat("_FillLevel", fillPercentage);
    }

    public void SetStamina(int stamina)
    {
        staminaSlider.value = stamina;

        float fillPercentage = staminaSlider.value / staminaSlider.maxValue;

        EnergyMaterial.SetFloat("_FillLevel", fillPercentage);
    }
}
