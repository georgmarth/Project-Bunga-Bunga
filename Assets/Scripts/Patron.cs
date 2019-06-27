using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PatronState { IDLE, WAITING, WALKING, ACTION}

[RequireComponent(typeof(Rigidbody))]
public class Patron : MonoBehaviour
{
    public PatronSettings PatronSettings;
    public NavMeshAgent Agent;
    public MeshRenderer WalkArea;
    public Animator Animator;
    public PatronState State { get; private set; }
    public GameEvents GameEvents;

    public PatronUI PatronUI;

    public int individualTip = 1;
    public int FinalTip = 1;

    private PatronLocation _desiredTask;
    private Rigidbody _rb;
    private bool _fulfilled = false;
    private int _completedTasks;
    private int _animWalkId;
    private int _animStopId;
    private Vector3 _spawnLocation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        State = PatronState.IDLE;
        _animWalkId = Animator.StringToHash("walk");
        _animStopId = Animator.StringToHash("stop");
    }

    private void Start()
    {
        _spawnLocation = transform.position;
        StartCoroutine(PatronLoop());
    }

    private IEnumerator PatronLoop()
    {
        while (_completedTasks < PatronSettings.DesiredTasks)
        {
            _fulfilled = false;

            // --- IDLE STATE ---

            State = PatronState.IDLE;
            for (int i = 0; i < PatronSettings.IdleLocations; i++)
            {
                var location = PickIdleLocation();
                Agent.enabled = true;
                Agent.SetDestination(location);
                Animator.SetTrigger("walk");
                yield return new WaitUntil(ReachedDestination);
                Agent.enabled = false;
                Animator.SetTrigger("stop");
                yield return new WaitForSeconds(Random.Range(PatronSettings.IdleTime.x, PatronSettings.IdleTime.y));
            }

            // --- IDLE OVER ---

            _desiredTask = PickTask();
            State = PatronState.WALKING;
            Agent.enabled = true;
            Agent.SetDestination(_desiredTask.Location.position);
            Animator.SetTrigger("walk");
            yield return new WaitUntil(ReachedDestination);

            // --- WAIT STATE ---

            State = PatronState.WAITING;
            // play animation
            Animator.SetTrigger(_desiredTask.TaskLocation.Task.PatronString);
            _rb.isKinematic = true;
            transform.SetPositionAndRotation(_desiredTask.Location.position, _desiredTask.Location.rotation);
            // init variables for loop
            float patienceRemaining = PatronSettings.Patience;
            float fulfillment = 0f;

            // reset Overhead UI
            PatronUI.SetPatienceActive(true);
            PatronUI.SetPatience(0f);
            PatronUI.SetFulfillmentActive(true);
            PatronUI.SetFulfillment(0f);

            while (patienceRemaining > 0f)
            {
                // set Overhead UI values
                PatronUI.SetPatience(Mathf.InverseLerp(PatronSettings.Patience, 0, patienceRemaining));
                PatronUI.SetFulfillment(Mathf.InverseLerp(0, _desiredTask.TaskLocation.Task.RequiredTime, fulfillment));

                bool fulfilling = _desiredTask.TaskLocation.Performing >= _desiredTask.TaskLocation.Task.RequiredPlayerCount
                    && (_desiredTask.TaskLocation.Task.RequiresPot ? _desiredTask.TaskLocation.UseLeft > 0f : true);
                if (fulfilling)
                {
                    fulfillment += Time.deltaTime;
                    if (fulfillment >= _desiredTask.TaskLocation.Task.RequiredTime)
                    {
                        _fulfilled = true;
                        _completedTasks++;
                        GameEvents.AddMoney(1);
                        break;
                    }
                }
                patienceRemaining -= Time.deltaTime;
                yield return null;
            }

            // close Overhead UI
            PatronUI.SetPatienceActive(false);
            PatronUI.SetFulfillmentActive(false);

            // get off position
            if (_desiredTask.ExitLocation != null)
            {
                transform.SetPositionAndRotation(_desiredTask.ExitLocation.position, _desiredTask.ExitLocation.rotation);
            }
            _rb.isKinematic = false;

            _desiredTask.Reserved = false;
            _desiredTask = null;

            // --- WAIT OVER ---

            if (!_fulfilled)
            {
                break;
            }
        }
        Agent.enabled = true;
        Agent.SetDestination(_spawnLocation);
        Animator.SetTrigger("walk");
        yield return new WaitUntil(ReachedDestination);

        if (_fulfilled)
        {
            // leave fulfilled
            GameEvents.AddMoney(1);
        }
        else
        {
            // leave frustrated
        }

        GameEvents.PatronLeft?.Invoke(this, _fulfilled);
        Destroy(gameObject, .5f);
    }

    private bool ReachedDestination()
    {
        return (Agent.destination - _rb.position).sqrMagnitude < Agent.stoppingDistance;
    }

    private PatronLocation PickTask()
    {
        var locations = FindObjectsOfType<TaskLocation>();
        int iterations = 0;
        do
        {
            iterations++;
            var location = locations[Random.Range(0, locations.Length)];
            for (int i = 0; i < location.PatronLocations.Length; i++)
            {
                if (!location.PatronLocations[i].Reserved)
                {
                    location.PatronLocations[i].Reserved = true;
                    return location.PatronLocations[i];
                }
            }
            if (iterations > 100)
            {
                Debug.LogError("Can't find a free location.");
                break;
            }
        } while (true);
        return null;
    }

    private Vector3 PickIdleLocation()
    {
        Vector3 location = new Vector3(
            Random.Range(WalkArea.bounds.min.x, WalkArea.bounds.max.x),
            Random.Range(WalkArea.bounds.min.y, WalkArea.bounds.max.y),
            Random.Range(WalkArea.bounds.min.z, WalkArea.bounds.max.z));
        return location;
    }
}
