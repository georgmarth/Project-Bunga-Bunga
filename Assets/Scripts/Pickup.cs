using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Collider Trigger;
    public Collider RigidCollider;

    public void Take()
    {
        Trigger.enabled = false;
        RigidCollider.enabled = false;
    }

    public void PutDown()
    {
        Trigger.enabled = true;
        RigidCollider.enabled = true;
    }
}
