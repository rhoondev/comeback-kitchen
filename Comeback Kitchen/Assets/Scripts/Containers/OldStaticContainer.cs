// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// // A static container has a list of predefined positions and rotations
// // which may be occupied by the objects themselves
// public abstract class OldStaticContainer : Container
// {
//     [SerializeField] protected StaticContainerDataAsset containerDataAsset;

//     protected virtual void Awake()
//     {
//         // Objects which start in the object holder must also exist in the data asset, or issues may occur
//         foreach (Transform child in ObjectHolder)
//         {
//             TrackObject(child.GetComponent<ContainerObject>());
//         }
//     }

//     protected virtual void TrackObject(ContainerObject obj)
//     {
//         Objects.Add(obj);
//         obj.Container = this;
//     }

//     protected void HandleRestoreRequest(ContainerObject obj)
//     {
//         if (CanRestoreObject(obj))
//         {
//             RestoreObject(obj);
//         }
//         else
//         {
//             obj.OnRestoreDenied();
//         }
//     }

//     protected abstract bool CanRestoreObject(ContainerObject obj);
//     protected abstract int GetRestoreIndex(ContainerObject obj);

//     protected virtual void RestoreObject(ContainerObject obj)
//     {
//         obj.transform.SetParent(ObjectHolder);

//         ObjectData data = _objectData[GetRestoreIndex(obj)];
//         obj.transform.SetLocalPositionAndRotation(data.position, data.rotation);

//         obj.RequestRestore.Clear();
//         obj.RequestTransfer.Clear();

//         obj.OnRestore();
//     }
// }
