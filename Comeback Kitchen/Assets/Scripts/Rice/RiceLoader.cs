using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(RiceContainer))]
public class RiceLoader : MonoBehaviour
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

    [SerializeField] private GameObject ricePrefab;
    [SerializeField] private Transform riceJar;
    [SerializeField] private bool loadOnStart;

    private RiceContainer _riceContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _riceContainer = GetComponent<RiceContainer>();

        if (loadOnStart)
        {
            LoadRice();
        }
    }

    private string GetSavePath() => Path.Combine(Application.persistentDataPath, "riceData.json");

    public void SaveRice()
    {
        RiceSaveData data = new RiceSaveData();

        foreach (var grain in _riceContainer.Grains)
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
            GameObject riceInstance = Instantiate(ricePrefab, riceJar.TransformPoint(rice.position), rice.rotation * riceJar.rotation);
            riceInstance.transform.SetParent(riceJar);
            _riceContainer.AddGrain(riceInstance);
        }

        Debug.Log("Rice loaded.");
    }
}
