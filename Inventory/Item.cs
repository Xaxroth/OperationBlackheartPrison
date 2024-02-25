using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string ItemName;

    public Mutation mutation;

    public enum PickupObject
    {
        Flashbang,
        Ammo,
        Mutation
    }

    public PickupObject ObjectType;

    // Start is called before the first frame update
    void Start()
    {
        if (ObjectType == PickupObject.Mutation)
        {
            mutation = GetComponent<Mutation>();
            ItemName = mutation.mutationData.MutationName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
