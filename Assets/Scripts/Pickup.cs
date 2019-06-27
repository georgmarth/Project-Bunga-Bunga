using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
    public Collider Trigger;
    public Collider RigidCollider;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Take()
    {
        Trigger.enabled = false;
        RigidCollider.enabled = false;
        _rb.isKinematic = true;
    }

    public void PutDown()
    {
        Trigger.enabled = true;
        RigidCollider.enabled = true;
        _rb.isKinematic = false;
    }
}
