using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTraps : MonoBehaviour
{
    public SpikeTrapScript[] SpikeTrapsToDeactivate;

    public void PullLever()
    {
        for (int i = 0; i < SpikeTrapsToDeactivate.Length; i++)
        {
            SpikeTrapsToDeactivate[i].enabled = false;
        }
    }

}
