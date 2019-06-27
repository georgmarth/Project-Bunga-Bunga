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

    public enum PlayerState { IDLE, INTERACTING, HOLDING, PLAYERSELECT, ANIMATION}

    public float MovementForce = 50f;
    public float StoppingForce = 2f;
    public float BoostForce = 100f;
    public float MaxSpeed = 5f;
    public float MaxBoostSpeed = 10f;
    public float BoostTime = .2f;
    public float BoostCooldown = 1f;
    public float RotationSpeed = 0.9f;
    public float Gravity = -9.81f;
    public float MaxCarrySpeed = 3f;

    public float RotationDeadZone = .05f;
    public float MovementDeadZone = .1f;

    public PlayerState State;

    public PlayerSwitcher PlayerSwitcher;
    public Animator Animator;
    public Transform HoldingHand;

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
    private TaskLocation _performingLocation;
    private Pickup _currentPickup;

    private ConsumeBox _consumeBox;

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
        if (State == PlayerState.IDLE || State == PlayerState.HOLDING)
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

            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, State == PlayerState.IDLE ? _speedLimit : MaxCarrySpeed);

            float sqrSpeedLimit = State == PlayerState.IDLE ? (_speedLimit * _speedLimit) : (MaxCarrySpeed * MaxCarrySpeed);
            Animator.SetFloat("walkspeed", _rb.velocity.sqrMagnitude / sqrSpeedLimit);

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
        if (!_dashing && State == PlayerState.IDLE)
        {
            _dashRoutine = StartCoroutine(DashRoutine());
        }
    }

    public void OnAnimationTimeSynchronization(float value)
    {
        Animator.SetFloat("synctime", value);
    }

    public void OnAction(InputValue inputValue)
    {
        if (inputValue.isPressed && _currentLocation != null && State == PlayerState.IDLE)
        {
            int performerNumber = _currentLocation.Performing;
            if (performerNumber < _currentLocation.Task.RequiredPlayerCount)
            {
                // interact with location without item
                State = PlayerState.INTERACTING;
                Animator.SetTrigger(_currentLocation.GetAnimationString());
                var animationTransform = _currentLocation.Locations[performerNumber];
                transform.SetPositionAndRotation(animationTransform.position, animationTransform.rotation);
                _currentLocation.Performing++;
                _rb.isKinematic = true;
                // hold item if the task has one
                if (_currentLocation.Task.ItemtoHold != null)
                {
                    Instantiate(_currentLocation.Task.ItemtoHold, HoldingHand);
                }
                if (_currentLocation.Task.Synchronized)
                {
                    _currentLocation.UpdateSynchronizedTime += OnAnimationTimeSynchronization;
                }
                _performingLocation = _currentLocation;
            }
        }
        else if (!inputValue.isPressed && State == PlayerState.INTERACTING)
        {
            // stop interacting
            State = PlayerState.IDLE;
            _performingLocation.Performing--;
            Animator.SetTrigger("walk");
            _rb.isKinematic = false;
            // Destroy all held items
            {
                foreach (Transform child in HoldingHand)
                {
                    Destroy(child.gameObject);
                }
            }
            if (_performingLocation.Task.Synchronized)
            {
                _performingLocation.UpdateSynchronizedTime -= OnAnimationTimeSynchronization;
            }
            _performingLocation = null;
        }
        else if (inputValue.isPressed && _currentPickup != null && State == PlayerState.IDLE)
        {
            // pick up item
            _currentPickup.Take();
            _currentPickup.transform.SetPositionAndRotation(HoldingHand.position, HoldingHand.rotation);
            _currentPickup.transform.SetParent(HoldingHand);
            _rb.isKinematic = true;
            State = PlayerState.ANIMATION;
            Animator.SetTrigger("lift");
        }
        else if (inputValue.isPressed && State == PlayerState.HOLDING)
        {
            if (_currentLocation != null && _currentLocation.Task.RequiresPot && _currentLocation.Performing == 0)
            {
                // use item on location
                foreach (Transform child in HoldingHand)
                {
                    Destroy(child.gameObject);
                }
                Animator.SetTrigger(_currentLocation.Task.PotString);
                _currentLocation.FillUp();
                _rb.isKinematic = true;
                var animationTransform = _currentLocation.Locations[0];
                transform.SetPositionAndRotation(animationTransform.position, animationTransform.rotation);
                State = PlayerState.ANIMATION;
            }
            else
            {
                // drop item
                var pickup = HoldingHand.GetChild(0).GetComponent<Pickup>();
                pickup.PutDown();
                HoldingHand.DetachChildren();
                Animator.SetTrigger("walk");
                State = PlayerState.IDLE;
            }
        }
        else if (inputValue.isPressed && State == PlayerState.IDLE && _consumeBox != null)
        {
            // pick up item
            Pickup pickup = Instantiate(_consumeBox.JugPrefab).GetComponent<Pickup>();
            pickup.Take();
            pickup.transform.SetPositionAndRotation(HoldingHand.position, HoldingHand.rotation);
            pickup.transform.SetParent(HoldingHand);
            _rb.isKinematic = true;
            State = PlayerState.ANIMATION;
            Animator.SetTrigger("lift");
        }
    }

    // triggered by AnimatorController
    public void AnimationEnd(PlayerState state)
    {
        State = state;
        _rb.isKinematic = false;
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

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(.1f);
        GameEvents.StartGame();
    }

    public void OnStartGame()
    {
        StartCoroutine(StartDelay());
    }

    private void OnGameStarted()
    {
        _pi.SwitchActions("CharacterAction");
    }

    private void OnPause()
    {
        if (GameEvents.GameState == GameState.RUNNING)
            GameEvents.PauseGame();
        else if (GameEvents.GameState == GameState.PAUSED)
            GameEvents.UnpauseGame();
    }

    private void OnQuit()
    {
        if (GameEvents.GameState == GameState.PAUSED || GameEvents.GameState == GameState.GAMEOVER)
        {
            Application.Quit();
        }
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
        if (other.tag == "TaskLocation")
        {
            TaskLocation location = other.GetComponent<TaskLocation>();
            if (location != null)
            {
                _currentLocation = location;
            }
        }
        else if (other.tag == "Pickup")
        {
            _currentPickup = other.GetComponent<Pickup>();
        }
        else if (other.tag == "PickupBox")
        {
            _consumeBox = other.GetComponent<ConsumeBox>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TaskLocation")
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
        else if (other.tag == "Pickup")
        {
            var pickup = other.GetComponent<Pickup>();
            if (_currentPickup == pickup)
            {
                _currentPickup = null;
            }
        }
        else if (other.tag == "PickupBox")
        {
            _consumeBox = null;
        }
    }
}
