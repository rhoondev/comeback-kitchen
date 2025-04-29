using UnityEngine;
using EzySlice;


//ChatGPT
public class SliceableObject : MonoBehaviour
{
    public bool isPinned = true;
    public Material cutMaterial;
    public float separationDistance = 0.01f;

    public void TrySlice(Vector3 slicePos, Vector3 sliceDirection)
    {
        SlicedHull hull = gameObject.Slice(slicePos, sliceDirection, cutMaterial);
        if (hull != null)
        {
            if(cutMaterial == null)
                cutMaterial = gameObject.GetComponent<Renderer>().material; //Ryan addition

            GameObject upper = hull.CreateUpperHull(gameObject, cutMaterial);
            GameObject lower = hull.CreateLowerHull(gameObject, cutMaterial);

            ApplySeparation(upper, lower, sliceDirection);

            if (!isPinned)
            {
                AddPhysics(upper);
                AddPhysics(lower);
            }

            Destroy(gameObject);
        }
    }


    // Separate pieces along the direction of the cut
    void ApplySeparation(GameObject upper, GameObject lower, Vector3 direction)
    {
        upper.transform.position += direction.normalized * separationDistance / 2f;
        lower.transform.position -= direction.normalized * separationDistance / 2f;
    }

    void AddPhysics(GameObject go)
    {
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 1f;
    }

    public void Unpin()
    {
        isPinned = false;
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = false;
    }
}
