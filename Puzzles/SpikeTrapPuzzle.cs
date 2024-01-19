using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapPuzzle : MonoBehaviour
{
    public SpikeTrapScript[] WallSpikes;
    public BoxCollider PlayerPlate;
    public BoxCollider TrapCollider;
    public GameObject Fire;
    public static bool SpikeTrialPassed;

    public void Update()
    {
        if (PlayerPlate.bounds.Contains(PlayerControllerScript.Instance.gameObject.transform.position))
        {
            SpikeTrialPassed = true;
            Fire.SetActive(true);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flashbang") || other.CompareTag("Player") || other.CompareTag("Natia"))
        {
            for (int i = 0; i < WallSpikes.Length; i++)
            {
                WallSpikes[i].Activate();

                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<PlayerControllerScript>().TakeDamage(100);
                }

                if (other.gameObject.CompareTag("Natia"))
                {
                    other.gameObject.GetComponent<Natia>().TakeDamage(100);
                }

            }

            StartCoroutine(TrapCooldown());
        }
    }
    private IEnumerator TrapCooldown()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.TrapActivated, 1.0f);
        TrapCollider.enabled = false;
        yield return new WaitForSeconds(5);
        TrapCollider.enabled = true;
    }
}
