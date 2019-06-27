using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
    public Collider Trigger;
    public Collider[] Colliders;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Take()
    {
        Trigger.enabled = false;
        foreach (var collider in Colliders)
        {
            collider.enabled = false;
        }
        _rb.isKinematic = true;
    }

    public void PutDown()
    {
        Trigger.enabled = true;
        foreach (var collider in Colliders)
        {
            collider.enabled = true;
        }
        _rb.isKinematic = false;
    }
}
