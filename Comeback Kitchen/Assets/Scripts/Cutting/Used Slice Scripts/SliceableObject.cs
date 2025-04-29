using UnityEngine;
using EzySlice;
using System.Collections.Generic;


//ChatGPT
public class SliceableObject : MonoBehaviour
{
    public Material cutMaterial;
    public float separationDistance = 0.01f;
    public int DivisionCount {get; set;}

    [SerializeField] Rigidbody rb;

    public void Awake()
    {
        
    }

    public SmartAction<int> OnCreated = new SmartAction<int>();

    public List<GameObject> TrySlice(Vector3 slicePos, Vector3 sliceDirection)
    {
        if(cutMaterial == null)
            cutMaterial = gameObject.GetComponent<Renderer>().material; //Ryan addition





        SlicedHull hull = gameObject.Slice(slicePos, sliceDirection, cutMaterial);




        if (hull != null)
        {

            GameObject upper = hull.CreateUpperHull(gameObject, cutMaterial);
            SliceableObject upperSO = upper.GetComponent<SliceableObject>();
            // upperSO.DivisionCount = upperSO.DivisionCount + 1;      //TODO - Revisit later
            // upperSO.OnCreated.Invoke(upperSO.DivisionCount);
            // TODO -- revisit and fix


            GameObject lower = hull.CreateLowerHull(gameObject, cutMaterial);
            SliceableObject lowerSO = upper.GetComponent<SliceableObject>();
            // lowerSO.DivisionCount = lowerSO.DivisionCount + 1;      //TODO - Revisit later


            // upperSO.OnCreated.Invoke(lowerSO.DivisionCount);

            ApplySeparation(upper, lower, sliceDirection);

            List<GameObject> hullList = new List<GameObject> { upper, lower };

            // Destroy(gameObject);        //Destroy triggers at end of frame so return hullList will happen
            return hullList;
        }
        return null;
    }


    // Separate pieces along the direction of the cut
    void ApplySeparation(GameObject upper, GameObject lower, Vector3 direction)
    {
        upper.transform.position += direction.normalized * separationDistance / 2f;
        lower.transform.position -= direction.normalized * separationDistance / 2f;
    }


    public void Unpin()
    {
        rb.isKinematic = true;
        // foreach (var rb in GetComponentsInChildren<Rigidbody>())
        //     rb.isKinematic = false;
    }

    public void Pin()
    {
        rb.isKinematic = false;
    }
}
