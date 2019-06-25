using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameEvents GameEvents;

    public Transform[] SpawnPoints;

    private List<GameObject> _players = new List<GameObject>();

    private void Awake()
    {
        GameEvents.GameStarted += SpawnPlayers;
        GameEvents.PlayerJoined += OnPlayerJoin;
    }

    private void OnPlayerJoin(GameObject player)
    {
        _players.Add(player);
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            var player = _players[i];

            player.transform.position = SpawnPoints[i].transform.position;
            player.transform.rotation = SpawnPoints[i].transform.rotation;

            player.GetComponent<Rigidbody>().isKinematic = false;

            player.layer = gameObject.layer;
            foreach (var transform in player.GetComponentsInChildren<Transform>(true))
            {
                transform.gameObject.layer = gameObject.layer;
            }
        }
    }
}
