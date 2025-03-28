using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RiceContainer))]
public class RicePourer : MonoBehaviour
{
    [SerializeField] private float minPourAngle;
    [SerializeField] private float maxPourSpeed;
    [SerializeField] private float frameRate;

    private RiceContainer _riceContainer;

    // Awake is called once before the first execution of Start after the MonoBehaviour is created
    private void Awake()
    {
        _riceContainer = GetComponent<RiceContainer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(PourRoutine());
    }

    // Update is called once per frame
    private IEnumerator PourRoutine()
    {
        yield return new WaitForSeconds(1f); // Wait for rice to load

        while (!_riceContainer.IsEmpty)
        {
            float angle = Vector3.Angle(Vector3.up, transform.up);

            if (angle > minPourAngle)
            {
                int pourRate = (int)(maxPourSpeed / frameRate * (angle - minPourAngle) / (180f - minPourAngle));
                int grainsToPour = Mathf.Min(pourRate, _riceContainer.GrainCount);

                for (int i = 0; i < grainsToPour; i++)
                {
                    GameObject grain = _riceContainer.RemoveGrain();
                    grain.transform.SetParent(null);
                    grain.GetComponent<Collider>().enabled = true;
                    grain.GetComponent<Rigidbody>().isKinematic = false;
                }
            }

            yield return new WaitForSeconds(1f / frameRate);
        }
    }
}
