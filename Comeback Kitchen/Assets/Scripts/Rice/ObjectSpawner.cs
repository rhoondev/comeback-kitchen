using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Container container;
    [SerializeField] private GameObject prefab;
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
                    Mathf.Sin(theta) * Mathf.Cos(phi),  // X component
                    -Mathf.Cos(theta),                 // Y component (negative for downward)
                    Mathf.Sin(theta) * Mathf.Sin(phi)  // Z component
                );

                GameObject instance = Instantiate(prefab, spawnPosition, Random.rotation);
                instance.transform.SetParent(container.ObjectHolder);
                instance.GetComponent<Rigidbody>().linearVelocity = direction * initialSpeed;
                container.Objects.Add(instance);
            }
        }
    }
}
