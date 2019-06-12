using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { IDLE, INTERACTING, HOLDING }

    public float MovementSpeed = 5f;
    public float RotationSpeed = 0.9f;
    public float RotationDeadZone = .05f;

    public PlayerState State;

    private Rigidbody rb;
    private Vector3 MovementInput;

    private TaskLocation currentLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        State = PlayerState.IDLE;
    }

    void Update()
    {
        // INPUT
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        MovementInput = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);

        if (Input.GetButtonDown("Fire1") && (State == PlayerState.IDLE || State == PlayerState.HOLDING ) && currentLocation != null)
        {
            State = PlayerState.INTERACTING;
            PerformTask();
        }
    }

    private void PerformTask()
    {
        StartCoroutine(TestPerform());
    }

    private IEnumerator TestPerform()
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(1f);
        State = PlayerState.IDLE;
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TaskLocation location = other.GetComponent<TaskLocation>();
        if (location == null)
        {
            return;
        }
        currentLocation = location;
    }

    private void OnTriggerExit(Collider other)
    {
        TaskLocation location = other.GetComponent<TaskLocation>();
        if (location == null)
        {
            return;
        }
        if (currentLocation == location)
        {
            currentLocation = null;
        }
    }

    private void FixedUpdate()
    {
        if (State == PlayerState.IDLE)
        {
            // VELOCITY
            Vector3 movement = MovementInput * MovementSpeed;
            float vertical = rb.velocity.y;
            rb.velocity = movement;
            rb.velocity += new Vector3(0f, vertical, 0f);

            // ROTATION
            if (MovementInput.sqrMagnitude >= RotationDeadZone)
            {
                Vector3 lookDirection = MovementInput.normalized;

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), RotationSpeed));
            }
        }
    }
}
