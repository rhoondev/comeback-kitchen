using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(BoxCollider))]
public class PanLiquid : MonoBehaviour
{
    [SerializeField] private ParticleSystem steam;
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float simmerPoint;
    [SerializeField] private float boilingPoint;
    [SerializeField] private int maxVolume;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color oilColor;
    [SerializeField] private Color tomatoColor;

    public bool IsBoiling => _temperature >= boilingPoint;

    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;
    private float _maxSurfaceDisplacement;
    private float _maxBoilingSpeed;
    private float _maxFillHeight;

    private Dictionary<LiquidType, int> _contents;
    private float _temperature = 70f;
    private int _totalVolume = 1750;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();

        _contents = new Dictionary<LiquidType, int>
        {
            { LiquidType.Water, 1000 },
            { LiquidType.Oil, 250 },
            { LiquidType.Tomato, 500 },
        };

        _maxSurfaceDisplacement = _meshRenderer.material.GetFloat("_Surface_Displacement");
        _maxBoilingSpeed = _meshRenderer.material.GetFloat("_Boiling_Speed");
        _maxFillHeight = _meshRenderer.material.GetFloat("_Height");
    }

    public void Heat(float panTemp)
    {
        if (_totalVolume == 0) return;
        float rate = thermalConductivity * (panTemp - _temperature);
        _temperature += rate * Time.deltaTime;
    }

    public void Fill(LiquidType type, int amount)
    {
        if (_totalVolume + amount > maxVolume)
        {
            amount = maxVolume - _totalVolume;
        }

        if (amount > 0)
        {
            _contents[type] += amount;
            _totalVolume += amount;
            _temperature = Mathf.Lerp(_temperature, 0f, (float)amount / _totalVolume);
        }
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

        float waterAmount = (float)_contents[LiquidType.Water] / _totalVolume;
        float oilAmount = (float)_contents[LiquidType.Oil] / _totalVolume;
        float tomatoAmount = (float)_contents[LiquidType.Tomato] / _totalVolume;

        Color color = waterColor * waterAmount +
                     oilColor * oilAmount +
                     tomatoColor * tomatoAmount;
        _meshRenderer.material.SetColor("_Color", color);

        float surfaceDisplacement = _maxSurfaceDisplacement * Mathf.InverseLerp(simmerPoint, boilingPoint, _temperature);
        _meshRenderer.material.SetFloat("_Surface_Displacement", surfaceDisplacement);

        float boilingSpeed = Mathf.Lerp(_maxBoilingSpeed / 2f, _maxBoilingSpeed, Mathf.InverseLerp(simmerPoint, boilingPoint, _temperature));
        _meshRenderer.material.SetFloat("_Boiling_Speed", boilingSpeed);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out Stream stream))
        {
            if (_totalVolume < maxVolume)
            {
                var collisionEvents = new List<ParticleCollisionEvent>();
                other.GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
                int collisionCount = collisionEvents.Count;

                Fill(stream.Type, collisionCount);

                float fillAmount = (float)_totalVolume / maxVolume;
                float fillHeight = _maxFillHeight * fillAmount;

                _meshRenderer.material.SetFloat("_Fill_Height", fillHeight);
                _boxCollider.size = new Vector3(_boxCollider.size.x, fillHeight, _boxCollider.size.z);
                _boxCollider.center = new Vector3(_boxCollider.center.x, fillHeight / 2f, _boxCollider.center.z);

                Debug.Log(_totalVolume);
            }
        }
    }
}
