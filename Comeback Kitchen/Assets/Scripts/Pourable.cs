using UnityEngine;

[RequireComponent(typeof(Liquid))]
public class Pourable : MonoBehaviour
{
    private Liquid _liquid;

    private void Start()
    {
        _liquid = GetComponent<Liquid>();
    }

    private void Update()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            _liquid.Drain(1);
        }
    }
}
