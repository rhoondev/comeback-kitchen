using UnityEngine;

[RequireComponent(typeof(RiceContainer))]
public class RicePourer : MonoBehaviour
{
    [SerializeField] private float minPourAngle;
    [SerializeField] private float maxPourSpeed;

    private RiceContainer _riceContainer;

    // Awake is called once before the first execution of Start after the MonoBehaviour is created
    private void Awake()
    {
        _riceContainer = GetComponent<RiceContainer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_riceContainer.IsEmpty)
        {
            return;
        }

        float angle = Vector3.Angle(Vector3.up, transform.up);

        if (angle > minPourAngle)
        {
            int pourRate = (int)(maxPourSpeed * (angle - minPourAngle) / (180f - minPourAngle));

            for (int i = 0; i < pourRate && !_riceContainer.IsEmpty; i++)
            {
                GameObject grain = _riceContainer.RemoveGrain();
                grain.transform.SetParent(null);
                grain.GetComponent<Collider>().enabled = true;
                grain.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }
}
