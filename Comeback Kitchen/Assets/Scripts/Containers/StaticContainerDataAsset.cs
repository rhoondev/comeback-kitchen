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

[CreateAssetMenu(fileName = "StaticContainerDataAsset", menuName = "Custom/Static Container Data Asset")]
public class StaticContainerDataAsset : ScriptableObject
{
    public List<ObjectData> objectData = new List<ObjectData>();
}