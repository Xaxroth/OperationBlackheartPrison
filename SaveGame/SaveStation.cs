using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class SaveStation : MonoBehaviour
{
    public ParticleSystem SaveGameParticles;

    public Shrine ShrineType;

    void Start()
    {
        int enumLength = System.Enum.GetValues(typeof(Shrine)).Length;
        int randomIndex = Random.Range(0, enumLength);

        ShrineType = (Shrine)randomIndex;
    }

    void Update()
    {
        
    }

    public void PlayParticles()
    {
        StartCoroutine(ParticlesCoroutine());
        ActivateShrine();
    }

    public void ActivateShrine()
    {
        UIManager.Instance.DisplayShrineMessage(ShrineType);
    }

    private IEnumerator ParticlesCoroutine()
    {
        SaveGameParticles.Play();
        yield return new WaitForSeconds(1);
        SaveGameParticles.Stop();
    }
}
