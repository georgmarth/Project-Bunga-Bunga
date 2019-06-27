using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatronSpawner : MonoBehaviour
{
    public GameObject[] PatronPrefabs;
    public float SpawnRate = 10f;
    public int MaxPatrons = 15;
    public int TotalPatrons = 30;

    public GameEvents GameEvents;
    public MeshRenderer PatronWalkArea;

    private int _numPatrons;
    private WaitForSeconds _spawnWait;

    private Coroutine _wave;
    private bool _paused = false;

    private void Awake()
    {
        GameEvents.PatronLeft += (patron, happy) => _numPatrons--;
        GameEvents.PatronSpawned += patron => _numPatrons++;
        GameEvents.GameStarted += OnGameStart;
        _spawnWait = new WaitForSeconds(SpawnRate);
    }

    public void OnGameStart()
    {
        _wave = StartCoroutine(SpawnWave());
    }

    public void OnPause()
    {
        _paused = true;
    }

    public void OnUnPause()
    {
        _paused = false;
    }

    public Patron SpawnPatron()
    {
        int index = Random.Range(0, PatronPrefabs.Length);
        var patronObject = Instantiate(PatronPrefabs[index], transform.position, transform.rotation);
        Patron patron = patronObject.GetComponent<Patron>();
        patron.GameEvents = GameEvents;
        patron.WalkArea = PatronWalkArea;
        GameEvents.PatronSpawned?.Invoke(patron);
        return patron;
    }

    IEnumerator SpawnWave()
    {
        int totalSpawns = 0;
        while (totalSpawns < TotalPatrons)
        {
            if (_paused)
                yield return new WaitUntil(() => !_paused);
            while (_numPatrons >= MaxPatrons)
            {
                yield return null;
            }
            SpawnPatron();
            totalSpawns++;
            yield return _spawnWait;
        }
    }

}
