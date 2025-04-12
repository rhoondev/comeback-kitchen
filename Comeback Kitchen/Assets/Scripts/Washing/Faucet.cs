using UnityEngine;

public class Faucet : MonoBehaviour
{
    [SerializeField] private ParticleSystem stream;
    [SerializeField] private Transform lever;
    [SerializeField] private float maxFlowRate;

    private float _flowRate;

    // Update is called once per frame
    private void Update()
    {
        _flowRate = lever.localEulerAngles.z / 30f * maxFlowRate;

        var emission = stream.emission;
        emission.rateOverTimeMultiplier = _flowRate;
    }
}
