using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RiceSpawner : MonoBehaviour
{
    [System.Serializable]
    class RiceData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    class RiceSaveData
    {
        public List<RiceData> riceGrains = new();
    }

    [SerializeField] private Transform jar;
    [SerializeField] private GameObject ricePrefab;
    [SerializeField] private int grainCount;
    [SerializeField] private int spawnRate;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float spawnAngle;
    [SerializeField] private float initialSpeed;
    [SerializeField] private bool spawnRice;
    [SerializeField] private bool loadRice;

    private List<GameObject> _riceGrains;
    private float _maxAngleRad;

    private void Awake()
    {
        _maxAngleRad = spawnAngle * Mathf.Deg2Rad;
        _riceGrains = new List<GameObject>();
    }

    private void Start()
    {
        if (loadRice)
        {
            LoadRice();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (spawnRice)
        {
            for (int i = 0; i < spawnRate && _riceGrains.Count < grainCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = transform.position + new Vector3(offset.x, 0f, offset.y);

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

                GameObject rice = Instantiate(ricePrefab, spawnPosition, Random.rotation);
                rice.GetComponent<Rigidbody>().linearVelocity = direction * initialSpeed;
                _riceGrains.Add(rice);
            }
        }
    }

    private string GetSavePath() => Path.Combine(Application.persistentDataPath, "riceData.json");

    public void SaveRice()
    {
        RiceSaveData data = new();
        foreach (var grain in _riceGrains)
        {
            data.riceGrains.Add(new RiceData
            {
                position = grain.transform.localPosition,
                rotation = grain.transform.localRotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("Rice saved to " + GetSavePath());
    }

    public void LoadRice()
    {
        string path = GetSavePath();
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        RiceSaveData data = JsonUtility.FromJson<RiceSaveData>(json);

        foreach (var rice in data.riceGrains)
        {
            GameObject riceInstance = Instantiate(ricePrefab, jar.TransformPoint(rice.position), rice.rotation * jar.rotation);
            riceInstance.transform.SetParent(jar);
            riceInstance.GetComponent<Rigidbody>().isKinematic = true;
            riceInstance.GetComponent<Collider>().enabled = false;
        }

        Debug.Log("Rice loaded.");
    }
}
