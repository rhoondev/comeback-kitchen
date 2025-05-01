using UnityEngine;

public class FoodItemReleaser : MonoBehaviour
{
    [SerializeField] private DynamicContainer container;
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