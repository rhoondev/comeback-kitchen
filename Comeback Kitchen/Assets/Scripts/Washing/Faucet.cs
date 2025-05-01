using UnityEngine;

public class Faucet : MonoBehaviour
{
    [SerializeField] private ParticleSystem stream;
    [SerializeField] private Transform lever;
    [SerializeField] private float maxFlowRate;

    public SmartAction OnTurnedFullyOn = new SmartAction();
    public SmartAction OnTurnedFullyOff = new SmartAction();

    private float _flowRate;
    private bool _turnedFullyOn = false;
    private bool _turnedFullyOff = true;

    // Update is called once per frame
    private void Update()
    {
        float angle = lever.localEulerAngles.z;

        if (angle > 180f)
        {
            angle -= 360f;
        }

        angle = Mathf.Clamp(angle, 0f, 30f);

        lever.localEulerAngles = new Vector3(lever.localEulerAngles.x, lever.localEulerAngles.y, angle);

        if (angle == 30f && !_turnedFullyOn)
        {
            _turnedFullyOn = true;
            _turnedFullyOff = false;
            OnTurnedFullyOn.Invoke();
        }
        else if (angle == 0f && !_turnedFullyOff)
        {
            _turnedFullyOff = true;
            _turnedFullyOn = false;
            OnTurnedFullyOff.Invoke();
        }
        else if (angle > 0f && angle < 30f)
        {
            _turnedFullyOn = false;
            _turnedFullyOff = false;
        }

        _flowRate = angle / 30f * maxFlowRate;

        var emission = stream.emission;
        emission.rateOverTimeMultiplier = _flowRate;
    }
}
