using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public GameEvents GameEvents;

    public enum PlayerState { IDLE, INTERACTING, HOLDING, PLAYERSELECT }

    public float MovementForce = 50f;
    public float StoppingForce = 2f;
    public float BoostForce = 100f;
    public float MaxSpeed = 5f;
    public float MaxBoostSpeed = 10f;
    public float BoostTime = .2f;
    public float BoostCooldown = 1f;
    public float RotationSpeed = 0.9f;
    public float Gravity = -9.81f;

    public float RotationDeadZone = .05f;
    public float MovementDeadZone = .1f;

    public PlayerState State;

    public PlayerSwitcher PlayerSwitcher;

    public int PlayerIndex { get; set; }

    private Rigidbody _rb;
    private PlayerInput _pi;
    private Vector3 _movementInput;
    private float _speedLimit;

    // Dash variables
    private bool _dashing;
    private Coroutine _dashRoutine;
    private WaitForSeconds _boostTimeWait;
    private WaitForSeconds _boostCooldownWait;

    private TaskLocation _currentLocation;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pi = GetComponent<PlayerInput>();
        State = PlayerState.IDLE;
        _speedLimit = MaxSpeed;

        _boostTimeWait = new WaitForSeconds(BoostTime);
        _boostCooldownWait = new WaitForSeconds(BoostCooldown);
        _dashing = false;

        GameEvents.GameStarted += OnGameStarted;
    }

    private void Start()
    {
        PlayerSwitcher.PlayerIndex = PlayerIndex;
        PlayerSwitcher.Shuffle();
    }

    private void FixedUpdate()
    {
        if (State == PlayerState.IDLE)
        {
            // VELOCITY
            Vector3 movement = _movementInput * MovementForce;
            float yComponent = _rb.velocity.y;
            yComponent += Gravity * Time.deltaTime;
            Vector3 oldVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.velocity = oldVelocity + movement * Time.deltaTime;

            // stop force
            if (_movementInput.sqrMagnitude <= MovementDeadZone * MovementDeadZone)
            {
                _rb.velocity += (_rb.velocity * -StoppingForce * Time.deltaTime);
            }

            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _speedLimit);
            _rb.velocity = new Vector3(_rb.velocity.x, yComponent, _rb.velocity.z);
            // ROTATION
            if (_movementInput.sqrMagnitude >= RotationDeadZone * RotationDeadZone)
            {
                Vector3 lookDirection = _movementInput.normalized;

                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), RotationSpeed));
            }
        }
    }

    // -- MOVEMENT --

    public void OnMove(InputValue value)
    {
        var v = value.Get<Vector2>();
        _movementInput = new Vector3(v.x, 0f, v.y);
    }

    public void OnDash()
    {
        if (!_dashing)
        {
            _dashRoutine = StartCoroutine(DashRoutine());
        }
    }

    public void OnAction(InputValue inputValue)
    {
        if (inputValue.isPressed && _currentLocation != null)
        {
            State = PlayerState.INTERACTING;
            _currentLocation.Performing = true;
            _rb.isKinematic = true;
        }
        else if (!inputValue.isPressed && State == PlayerState.INTERACTING)
        {
            State = PlayerState.IDLE;
            _currentLocation.Performing = false;
            _rb.isKinematic = false;
        }
    }

    // -- CHARACTER SELECTION

    public void OnSwitchOutfitLeft()
    {
        PlayerSwitcher.Switch(false);
    }

    public void OnSwitchOutfitRight()
    {
        PlayerSwitcher.Switch(true);
    }

    public void OnSwitchOutfitUp()
    {
        PlayerSwitcher.SwitchOption(true);
    }

    public void OnSwitchOutfitDown()
    {
        PlayerSwitcher.SwitchOption(false);
    }

    public void OnStartGame()
    {
        GameEvents.StartGame();
    }

    private void OnGameStarted()
    {
        _pi.SwitchActions("CharacterAction");
    }

    private IEnumerator DashRoutine()
    {
        _dashing = true;
        _rb.AddForce(transform.forward * BoostForce, ForceMode.Impulse);
        _speedLimit = MaxBoostSpeed;
        yield return _boostTimeWait;
        _speedLimit = MaxSpeed;
        yield return _boostCooldownWait;
        _dashing = false;
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
}
