using UnityEngine;

public class Faucet : MonoBehaviour
{
    [SerializeField] private ParticleSystem stream;

    private float flowRate;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            flowRate += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            flowRate -= Time.deltaTime;
        }

        if (flowRate < 0f)
        {
            flowRate = 0f;
        }

        var emission = stream.emission;
        emission.rateOverTimeMultiplier = flowRate;

    }
}
