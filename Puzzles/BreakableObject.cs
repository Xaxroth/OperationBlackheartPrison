using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int Health = 50;
    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
