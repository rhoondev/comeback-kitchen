using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PanLiquid : MonoBehaviour
{
    [SerializeField] private ParticleSystem steam;
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float simmerPoint;
    [SerializeField] private float boilingPoint;

    public bool IsBoiling => _temperature >= boilingPoint;

    private MeshRenderer _meshRenderer;
    private float _maxSurfaceDisplacement;
    private float _maxBoilingSpeed;
    private float _temperature = 0f;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _maxSurfaceDisplacement = _meshRenderer.material.GetFloat("_Surface_Displacement");
        _maxBoilingSpeed = _meshRenderer.material.GetFloat("_Boiling_Speed");

    }

    private void Update()
    {
        if (IsBoiling)
        {
            if (!steam.isPlaying)
            {
                steam.Play();
            }
        }
        else
        {
            if (steam.isPlaying)
            {
                steam.Stop();
            }
        }

        _meshRenderer.material.SetFloat("_Surface_Displacement", _maxSurfaceDisplacement * Mathf.InverseLerp(simmerPoint, boilingPoint, _temperature));
        _meshRenderer.material.SetFloat("_Boiling_Speed", Mathf.Lerp(_maxBoilingSpeed / 2f, _maxBoilingSpeed, Mathf.InverseLerp(simmerPoint, boilingPoint, _temperature)));
    }

    public void Heat(float panTemp)
    {
        float rate = thermalConductivity * (panTemp - _temperature);
        _temperature += rate * Time.deltaTime;
    }
}
