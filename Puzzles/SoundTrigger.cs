using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioSource SoundSource;
    public AudioClip SoundToPlay;
    public bool SoundPlayed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!SoundPlayed && other.CompareTag("Player"))
        {
            SoundSource.PlayOneShot(SoundToPlay, 2.0f);
            SoundPlayed = true;
        }

    }
}
