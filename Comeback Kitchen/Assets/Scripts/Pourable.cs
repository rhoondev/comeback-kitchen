using UnityEngine;

[RequireComponent(typeof(Liquid))]
public class Pourable : MonoBehaviour
{
    [SerializeField] private float openingSize;
    [SerializeField] private float maxPourSpeed;
    [SerializeField] private ParticleSystem stream;
    [SerializeField] private float particlePourSpeedMultiplier;

    private Liquid _liquid;

    private void Start()
    {
        _liquid = GetComponent<Liquid>();
    }

    private void Update()
    {
        if (!_liquid.IsEmpty && _liquid.FillCutoff > GetPourHeight())
        {
            float angle = Vector3.Angle(Vector3.up, transform.up);
            int pourSpeed = (int)(maxPourSpeed * angle / 180f);
            _liquid.Drain(pourSpeed);

            var emission = stream.emission;
            emission.rateOverTime = pourSpeed * particlePourSpeedMultiplier;
        }
        else
        {
            var emission = stream.emission;
            emission.rateOverTime = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.IsPlaying(this))
        {
            // Vector3 start = transform.TransformPoint(new Vector3(-openingSize / 2f, _liquid.MaxFillHeight, 0f));
            // Vector3 end = transform.TransformPoint(new Vector3(openingSize / 2f, _liquid.MaxFillHeight, 0f));
            // Gizmos.DrawLine(start, end);

            float pourHeight = GetPourHeight();
            Vector3 start = new Vector3(transform.position.x - 0.5f, pourHeight, transform.position.z);
            Vector3 end = new Vector3(transform.position.x + 0.5f, pourHeight, transform.position.z);
            Gizmos.DrawLine(start, end);

            Vector3 start2 = new Vector3(transform.position.x - 0.5f, _liquid.FillCutoff, transform.position.z);
            Vector3 end2 = new Vector3(transform.position.x + 0.5f, _liquid.FillCutoff, transform.position.z);
            Gizmos.DrawLine(start2, end2);
        }
    }

    private float GetPourHeight()
    {
        // Compute the basis vectors in the plane (with magnitude equal to half the opening size)
        Vector3 v1 = transform.TransformVector(Vector3.right * openingSize / 2f);
        Vector3 v2 = transform.TransformVector(Vector3.forward * openingSize / 2f);

        // The y-components of the basis vectors
        float a = v1.y;
        float b = v2.y;

        // Compute the angle that minimizes the y-component.
        // The y component of any vector in the plane is: a*cos(theta) + b*sin(theta)
        // Its minimum occurs at theta_min = arctan2(b, a) + PI.
        float thetaMin = Mathf.Atan2(b, a) + Mathf.PI;

        // Construct the vector at this optimal angle
        Vector3 offset = v1 * Mathf.Cos(thetaMin) + v2 * Mathf.Sin(thetaMin);

        // Find the center of the top of the container
        Vector3 center = transform.TransformPoint(Vector3.up * _liquid.MaxFillHeight);

        // Return the y-component of the pour point
        return (center + offset).y;
    }
}
