using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit UnitData;
    private Rigidbody Rigidbody;
    void Start()
    {
        UnitData = GetComponent<Unit>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Rigidbody.AddForce(transform.forward * UnitData.movementRange / 20, ForceMode.Impulse);
    }
}
