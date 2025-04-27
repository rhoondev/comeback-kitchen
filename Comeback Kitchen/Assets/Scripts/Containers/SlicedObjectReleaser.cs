using UnityEngine;

public class SlicedObjectReleaser : MonoBehaviour
{
    [SerializeField] private UnorderedStaticContainer container;
    [SerializeField] private float releaseAngle;

    private void Update()
    {
        float angle = Vector3.Angle(Vector3.up, transform.up);

        if (angle >= releaseAngle)
        {
            foreach (var obj in container.Objects)
            {
                container.ReleaseObject(obj);
            }
        }
    }
}