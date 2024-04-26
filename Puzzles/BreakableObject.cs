using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int Health = 50;
    public GameObject DestructionEffect;
    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            GameObject Destruction = Instantiate(DestructionEffect, transform.position, Quaternion.identity);
            Destroy(Destruction, 3f);   
            Destroy(gameObject);
        }
    }
}
