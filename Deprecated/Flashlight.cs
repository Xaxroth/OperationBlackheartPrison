using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private bool _active = true;
    [SerializeField] private GameObject FlashlightGO;

    [SerializeField] private AudioClip _flashOn;
    [SerializeField] private AudioClip _flashOff;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !_active)
        {
            _active = true;
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(_flashOn);
            FlashlightGO.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.F) && _active)
        {
            _active = false;
            PlayerControllerScript.Instance.PlayerAudioSource.PlayOneShot(_flashOff);
            FlashlightGO.SetActive(false);
        }
    }
}
