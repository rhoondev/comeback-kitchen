using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Flame : MonoBehaviour
{
    private ParticleSystem _ps;
    private float _maxLifetime;

    public float Size { get; private set; }

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _maxLifetime = _ps.main.startLifetime.constantMax;
    }

    public void SetFlameSize(float size)
    {
        Size = size;

        var mainModule = _ps.main;
        mainModule.startLifetime = size * _maxLifetime;
    }
}
