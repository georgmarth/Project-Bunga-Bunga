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
    public Vector3 WalkAreaMin;
    public Vector3 WalkAreaMax;
    public Animator Animator;
    public PatronState State { get; private set; }
    public GameEvents GameEvents;

    public PatronUI PatronUI;

    public int individualTip = 1;
    public int FinalTip = 1;

    public AudioClip[] GrumpyClips;
    public AudioClip[] HappyClips;

    private PatronLocation _desiredTask;
    private Rigidbody _rb;
    private bool _fulfilled = false;
    private int _completedTasks;
    private int _animWalkId;
    private int _animStopId;
    private Vector3 _spawnLocation;

    public AudioSource AudioSource;

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

            _desiredTask = PickTask2();
            State = PatronState.WALKING;
            Agent.enabled = true;
            Agent.SetDestination(_desiredTask.Location.position);
            Animator.SetTrigger("walk");
            yield return new WaitUntil(ReachedDestination);

            // --- WAIT STATE ---

            State = PatronState.WAITING;
            // play animation
            Agent.enabled = false;
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
            transform.SetPositionAndRotation(_desiredTask.Location.position, _desiredTask.Location.rotation);
            Animator.SetTrigger(_desiredTask.TaskLocation.Task.PatronString);
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
                        //play Sound
                        AudioSource.clip = HappyClips[Random.Range(0, HappyClips.Length)];
                        AudioSource.Play();

                        GameEvents.TaskFulfilled?.Invoke(this);
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
                //play Sound
                AudioSource.clip = GrumpyClips[Random.Range(0, GrumpyClips.Length)];
                AudioSource.Play();
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
            //play Sound
            AudioSource.clip = HappyClips[Random.Range(0, HappyClips.Length)];
            AudioSource.Play();
        }
        else
        {
            //play Sound
            AudioSource.clip = GrumpyClips[Random.Range(0, GrumpyClips.Length)];
            AudioSource.Play();
        }

        GameEvents.PatronLeft?.Invoke(this, _fulfilled);
        Destroy(gameObject, .5f);
    }

    private bool ReachedDestination()
    {
        return (Agent.destination - _rb.position).sqrMagnitude < Agent.stoppingDistance;
    }

    private PatronLocation PickTask2()
    {
        var locations = FindObjectsOfType<TaskLocation>();
        List<PatronLocation> freeLocations = new List<PatronLocation>();
        foreach (var location in locations)
        {
            foreach (var patronLocation in location.PatronLocations)
            {
                if (!patronLocation.Reserved)
                {
                    freeLocations.Add(patronLocation);
                }
            }
        }
        var desiredLocation = freeLocations[Random.Range(0, freeLocations.Count)];
        desiredLocation.Reserved = true;

        return desiredLocation;
    }

    private Vector3 PickIdleLocation()
    {
        Vector3 location = new Vector3(
            Random.Range(WalkAreaMin.x,WalkAreaMax.x),
            Random.Range(WalkAreaMin.y, WalkAreaMax.y),
            Random.Range(WalkAreaMin.z, WalkAreaMax.z));
        NavMeshHit hit;
        float distance = (location - transform.position).magnitude;
        NavMesh.SamplePosition(location, out hit, distance, 1);
        return hit.position;
    }
}
