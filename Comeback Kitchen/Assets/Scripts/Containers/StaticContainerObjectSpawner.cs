using UnityEngine;

public class StaticContainerObjectSpawner : MonoBehaviour
{
    [SerializeField] private StaticContainerDataHandler containerDataHandler;
    [SerializeField] private Transform objectHolder;
    [SerializeField] private Rigidbody objectPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int spawnRate;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float spawnAngle;
    [SerializeField] private float initialSpeed;
    [SerializeField] private bool isSpawning;

    private float _maxAngleRad;

    private void Awake()
    {
        _maxAngleRad = spawnAngle * Mathf.Deg2Rad;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isSpawning)
        {
            for (int i = 0; i < spawnRate; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, 0f, offset.y);

                // Generate a random theta (angle from the downward axis)
                float theta = Mathf.Acos(Random.Range(Mathf.Cos(_maxAngleRad), 1f));

                // Generate a random phi (rotation around the downward axis)
                float phi = Random.Range(0f, 2f * Mathf.PI);

                // Convert spherical coordinates to Cartesian
                Vector3 direction = new Vector3(
                    Mathf.Sin(theta) * Mathf.Cos(phi), // X component
                    -Mathf.Cos(theta),                 // Y component (negative for downward)
                    Mathf.Sin(theta) * Mathf.Sin(phi)  // Z component
                );

                Rigidbody rb = Instantiate(objectPrefab, spawnPosition, Random.rotation);
                rb.transform.SetParent(objectHolder);
                containerDataHandler.TrackedObjects.Add(rb.gameObject);

                rb.linearVelocity = direction * initialSpeed;
            }
        }
    }
}
