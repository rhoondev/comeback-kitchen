using System.Collections;
using UnityEngine;

public class RicePourer : MonoBehaviour
{
    [SerializeField] private StaticContainer container;
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
        while (true)
        {
            float angle = Vector3.Angle(Vector3.up, transform.up);

            if (angle > minPourAngle)
            {
                int pourRate = (int)(maxPourSpeed / frameRate * (angle - minPourAngle) / (180f - minPourAngle));

                for (int i = 0; i < pourRate; i++)
                {
                    container.ReleaseObject();
                }
            }

            yield return new WaitForSeconds(1f / frameRate);
        }
    }
}
