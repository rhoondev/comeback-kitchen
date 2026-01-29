using System.Collections.Generic;
using UnityEngine;

public class DynamicContainerTiltReleaser : MonoBehaviour
{
    [SerializeField] private DynamicContainer container;
    [SerializeField] private float releaseAngle;

    // Reusable list to avoid allocation every frame
    private readonly List<DynamicObject> _objectsToRelease = new List<DynamicObject>();

    private void Update()
    {
        float angle = Vector3.Angle(Vector3.up, transform.up);

        if (angle >= releaseAngle)
        {
            // Copy to temporary list to avoid modifying collection during iteration
            _objectsToRelease.Clear();
            _objectsToRelease.AddRange(container.Objects);

            foreach (var obj in _objectsToRelease)
            {
                container.ReleaseObject(obj);
            }
        }
    }
}