using UnityEngine;

public class Knob : MonoBehaviour
{
    [SerializeField] private ParticleSystem flame;

    private float _maxLifetime;
    private float _setting = 0f;

    private void Start()
    {
        _maxLifetime = flame.main.startLifetime.constantMax;
    }

    // Update is called once per frame
    private void Update()
    {
        float rotation = Mod(transform.eulerAngles.y, 360f); // Normalize the rotation to [0, 360]

        if (rotation == 0f)
        {
            _setting = 0f;
        }
        else
        {
            _setting = 1f - rotation / 360f;
        }

        var mainModule = flame.main;
        mainModule.startLifetime = _setting * _maxLifetime;
    }

    private float Mod(float a, float b)
    {
        return ((a % b) + b) % b;
    }
}
