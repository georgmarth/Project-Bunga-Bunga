using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PatronState { IDLE, WAITING, WALKING, ACTION}

public class Patron : MonoBehaviour
{
    public PatronSettings PatronSettings;
    public NavMeshAgent Agent;

    private int _completedTasks;
    private float _idleTimer;
    private PatronState _state;

    private PatronLocation _desiredTask;

    private void Start()
    {
        _state = PatronState.IDLE;
        _idleTimer = Random.Range(PatronSettings.IdleTime.x, PatronSettings.IdleTime.y);
    }

    private void Update()
    {
        if (_state == PatronState.IDLE)
        {
            _idleTimer -= Time.deltaTime;

            if (_idleTimer <= 0f)
            {
                _desiredTask = PickTask();
                _state = PatronState.WALKING;
                StartWalking();
            }
            else
            {

            }
        }
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

    private void StartWalking()
    {
        Agent.SetDestination(_desiredTask.Location.position);
        Agent.enabled = true;
    }
}
