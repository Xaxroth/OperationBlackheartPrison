using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStation : MonoBehaviour
{
    public ParticleSystem SaveGameParticles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayParticles()
    {
        StartCoroutine(ParticlesCoroutine());
    }

    private IEnumerator ParticlesCoroutine()
    {
        SaveGameParticles.Play();
        yield return new WaitForSeconds(1);
        SaveGameParticles.Stop();
    }
}
