using System.Collections;
using UnityEngine;

public class RicePourer : MonoBehaviour
{
    [SerializeField] private Container container;
    [SerializeField] private float minPourAngle;
    [SerializeField] private float maxPourSpeed;
    [SerializeField] private float frameRate;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(PourRoutine());
    }

    // Update is called once per frame
    private IEnumerator PourRoutine()
    {
        yield return new WaitForSeconds(1f); // Wait for rice to load

        while (!container.IsEmpty)
        {
            float angle = Vector3.Angle(Vector3.up, transform.up);

            if (angle > minPourAngle)
            {
                int pourRate = (int)(maxPourSpeed / frameRate * (angle - minPourAngle) / (180f - minPourAngle));
                int grainsToPour = Mathf.Min(pourRate, container.MaxObjectCount);

                for (int i = 0; i < grainsToPour; i++)
                {
                    container.ReleaseObject();
                }
            }

            yield return new WaitForSeconds(1f / frameRate);
        }
    }
}
