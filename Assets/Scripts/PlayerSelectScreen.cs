using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectScreen : MonoBehaviour
{
    public GameEvents GameEvents;

    public PlayerSelectStrip[] PlayerSelectStrips;
    public Transform[] PreviewPositions;

    int _currentPlayer = 0;

    private void Awake()
    {
        GameEvents.PlayerJoined += OnPlayerJoin;
    }

    private void OnPlayerJoin(GameObject playerObject)
    {
        PlayerSelectStrips[_currentPlayer].Join();
        playerObject.GetComponent<Rigidbody>().isKinematic = true;

        playerObject.transform.position = PreviewPositions[_currentPlayer].position;
        playerObject.transform.rotation = PreviewPositions[_currentPlayer].rotation;

        // set new layer
        int layer = PreviewPositions[_currentPlayer].gameObject.layer;
        playerObject.gameObject.layer = layer;
        foreach (var transform in playerObject.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = layer;
        }

        _currentPlayer++;
    }

    private void OnDestroy()
    {
        GameEvents.PlayerJoined -= OnPlayerJoin;
    }
}
