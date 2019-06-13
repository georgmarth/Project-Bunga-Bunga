using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { IDLE, INTERACTING, HOLDING }

    public float MovementForce = 50f;
    public float MaxSpeed = 5f;
    public float MaxBoostSpeed = 7f;
    public float RotationSpeed = 0.9f;
    public float RotationDeadZone = .05f;
    public float MovementDeadZone = .1f;

    public PlayerState State;

    private Rigidbody rb;
    private Vector3 _movementInput;
    private bool _boost;

    private TaskLocation _currentLocation;

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
        _movementInput = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);
        _boost = Input.GetButton("Fire2");

        if (Input.GetButtonDown("Fire1") && (State == PlayerState.IDLE || State == PlayerState.HOLDING ) && _currentLocation != null)
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
        for (int i = 0; i < _currentLocation.PatronLocations.Length; i++)
        {
            // task complete
            //_currentLocation.PatronLocations[i].Patron
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TaskLocation location = other.GetComponent<TaskLocation>();
        if (location == null)
        {
            return;
        }
        _currentLocation = location;
    }

    private void OnTriggerExit(Collider other)
    {
        TaskLocation location = other.GetComponent<TaskLocation>();
        if (location == null)
        {
            return;
        }
        if (_currentLocation == location)
        {
            _currentLocation = null;
        }
    }

    private void FixedUpdate()
    {
        if (State == PlayerState.IDLE)
        {
            // VELOCITY
            Vector3 movement = _movementInput * MovementForce;
            rb.AddForce(movement, ForceMode.Acceleration);

            // stop force
            if (_movementInput.sqrMagnitude <= MovementDeadZone * MovementDeadZone)
            {
                rb.AddForce(rb.velocity * -.2f, ForceMode.VelocityChange);
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, _boost ? MaxBoostSpeed : MaxSpeed);

            // ROTATION
            if (_movementInput.sqrMagnitude >= RotationDeadZone * RotationDeadZone)
            {
                Vector3 lookDirection = _movementInput.normalized;

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), RotationSpeed));
            }
        }
    }
}
