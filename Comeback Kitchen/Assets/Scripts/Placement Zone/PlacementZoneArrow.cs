using UnityEngine;

public class PlacementZoneArrow : MonoBehaviour
{
    [SerializeField] private float bounceHeight;
    [SerializeField] private float bounceSpeed;

    private Vector3 _originalPosition;

    private void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        float offset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.localPosition = _originalPosition + new Vector3(0f, offset, 0f);
    }
}
