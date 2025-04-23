using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSaver : MonoBehaviour
{
    [System.Serializable]
    class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    class TransformSaveData
    {
        public List<TransformData> objectData = new();
    }

    [SerializeField] private Container container;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string saveFileName;

    private string GetSavePath() => Path.Combine(Application.persistentDataPath, saveFileName);

    public void Save()
    {
        TransformSaveData data = new TransformSaveData();

        foreach (var obj in container.Objects)
        {
            data.objectData.Add(new TransformData
            {
                position = obj.transform.localPosition,
                rotation = obj.transform.localRotation
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("Rice saved to " + GetSavePath());
    }

    public void Load()
    {
        string path = GetSavePath();
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        TransformSaveData saveData = JsonUtility.FromJson<TransformSaveData>(json);

        foreach (var obj in saveData.objectData)
        {
            GameObject instance = Instantiate(prefab, container.ObjectHolder.TransformPoint(obj.position), obj.rotation * container.ObjectHolder.rotation);
            instance.transform.SetParent(container.ObjectHolder);
        }

        Debug.Log("Objects loaded.");
    }
}
