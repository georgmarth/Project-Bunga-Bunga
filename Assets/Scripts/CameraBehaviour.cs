using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public List<GameObject> Tracked;

    public float MinDistance = 10f;
    public float DistanceMultiplier = 1f;
    public float SmoothTime = 1f;

    public GameEvents GameEvents;

    private Vector3 _currentVelocity;

    private void Awake()
    {
        GameEvents.PlayerJoined += AddTracked;
        GameEvents.PlayerLeft += RemoveTracked;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentVelocity = new Vector3();
    }

    private void AddTracked(GameObject gameObject)
    {
        Tracked.Add(gameObject);
    }

    private void RemoveTracked(GameObject gameObject)
    {
        Tracked.Remove(gameObject);
    }

    private void OnDisable()
    {
        GameEvents.PlayerJoined -= AddTracked;
        GameEvents.PlayerLeft -= RemoveTracked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Tracked.Count > 0)
        {
            Vector2 minMaxHorizontal, minMaxVertical;
            minMaxHorizontal = minMaxVertical = new Vector2(float.MaxValue, float.MinValue);
            Vector3 sumPosition = new Vector3();

            for (int i = 0; i < Tracked.Count; i++)
            {
                Vector3 objectPosition = Tracked[i].transform.position;
                sumPosition += objectPosition;

                if (objectPosition.x < minMaxHorizontal.x)
                    minMaxHorizontal.x = objectPosition.x;
                if (objectPosition.x > minMaxHorizontal.y)
                    minMaxHorizontal.y = objectPosition.x;

                if (objectPosition.z < minMaxVertical.x)
                    minMaxVertical.x = objectPosition.z;
                if (objectPosition.z > minMaxVertical.y)
                    minMaxVertical.y = objectPosition.z;
            }

            Vector3 meanPosition = sumPosition / Tracked.Count;

            Vector2 dimensions = new Vector2(minMaxHorizontal.y - minMaxHorizontal.x, minMaxVertical.y - minMaxVertical.x);

            float addDistance = Mathf.Max(dimensions.x, dimensions.y) * DistanceMultiplier;
            float distance = MinDistance + addDistance;

            Vector3 forward = transform.forward;
            Vector3 newPosition = meanPosition - (forward * distance);
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _currentVelocity, SmoothTime);
        }
    }
}
