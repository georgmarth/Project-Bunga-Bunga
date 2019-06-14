using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PatronState { IDLE, WAITING, WALKING, ACTION}

public class Patron : MonoBehaviour
{
    public PatronSettings PatronSettings;
    public NavMeshAgent Agent;

    public MeshRenderer WalkArea;

    private int _completedTasks;
    private float _idleTimer;
    private PatronState _state;

    private PatronLocation _desiredTask;

    private void Start()
    {
        
        _idleTimer = Random.Range(PatronSettings.IdleTime.x, PatronSettings.IdleTime.y);
        StartCoroutine(Idle());
    }

    private IEnumerator Idle()
    {
        _state = PatronState.IDLE;
        for (int i = 0; i < PatronSettings.IdleLocations; i++)
        {
            var location = PickIdleLocation();
            Agent.enabled = true;
            Agent.SetDestination(location);
            yield return new WaitForSeconds(Random.Range(PatronSettings.IdleWalkTime.x, PatronSettings.IdleWalkTime.y));
            Agent.enabled = false;
            yield return new WaitForSeconds(Random.Range(PatronSettings.IdleTime.x, PatronSettings.IdleTime.y));
        }

        _desiredTask = PickTask();
        _state = PatronState.WALKING;

        Agent.enabled = true;
        Agent.SetDestination(_desiredTask.Location.position);
    }

    private void Update()
    {
        
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
