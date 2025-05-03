using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectData
{
    public Vector3 position;
    public Quaternion rotation;

    public ObjectData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public ObjectData Copy()
    {
        return new ObjectData(position, rotation);
    }
}

[CreateAssetMenu(fileName = "ContainerDataAsset", menuName = "Custom/Container Data Asset")]
public class ContainerDataAsset : ScriptableObject
{
    public List<ObjectData> objectData = new List<ObjectData>();
}