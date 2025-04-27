using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LiquidTemperature
{
    RoomTemperature,
    Simmering,
    Boiling
}

[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(BoxCollider))]
public class PanLiquid : MonoBehaviour
{
    [SerializeField] private ParticleSystem steam;
    [SerializeField] private int maxVolume;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color oilColor;
    [SerializeField] private Color tomatoColor;

    public SmartAction<Dictionary<LiquidType, int>> OnLiquidAdded = new SmartAction<Dictionary<LiquidType, int>>();
    public SmartAction<LiquidTemperature> OnTemperatureFullyChanged = new SmartAction<LiquidTemperature>();

    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;
    private float _maxSurfaceDisplacement;
    private float _maxBoilingSpeed;
    private float _maxFillHeight;

    private Dictionary<LiquidTemperature, float> _temperatureMap;
    private Dictionary<LiquidType, int> _contents;
    private float _temperature;
    private int _totalVolume;

    private bool IsBoiling { get => _temperature >= _temperatureMap[LiquidTemperature.Boiling]; }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();

        _contents = new Dictionary<LiquidType, int>
        {
            { LiquidType.Oil, 0 },
            { LiquidType.Water, 0 },
            { LiquidType.TomatoJuice, 0 }
        };

        _temperatureMap = new Dictionary<LiquidTemperature, float>
        {
            { LiquidTemperature.RoomTemperature, 70f },
            { LiquidTemperature.Simmering, 180f },
            { LiquidTemperature.Boiling, 212f }
        };

        _temperature = _temperatureMap[LiquidTemperature.RoomTemperature];

        _maxSurfaceDisplacement = _meshRenderer.material.GetFloat("_Surface_Displacement");
        _maxBoilingSpeed = _meshRenderer.material.GetFloat("_Boiling_Speed");
        _maxFillHeight = _meshRenderer.material.GetFloat("_Height");
    }

    public void ChangeTemperature(LiquidTemperature targetTemperature, float duration)
    {
        StartCoroutine(ModulateTemperature(targetTemperature, duration));
    }

    private IEnumerator ModulateTemperature(LiquidTemperature targetTemperature, float duration)
    {
        float startingTemperature = _temperature;
        float endingTemperature = _temperatureMap[targetTemperature];
        float timePassed = 0f;

        while (timePassed < duration)
        {
            float t = timePassed / duration;
            _temperature = Mathf.Lerp(startingTemperature, endingTemperature, t);
            timePassed += Time.deltaTime;
            yield return null;
        }

        _temperature = endingTemperature;
        OnTemperatureFullyChanged.Invoke(targetTemperature);
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

            OnLiquidAdded.Invoke(_contents);
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
        float tomatoAmount = (float)_contents[LiquidType.TomatoJuice] / _totalVolume;

        Color color = waterColor * waterAmount +
                      oilColor * oilAmount +
                      tomatoColor * tomatoAmount;

        _meshRenderer.material.SetColor("_Color", color);

        float boilAmount = Mathf.InverseLerp(_temperatureMap[LiquidTemperature.Simmering], _temperatureMap[LiquidTemperature.Boiling], _temperature);
        _meshRenderer.material.SetFloat("_Surface_Displacement", _maxSurfaceDisplacement * boilAmount);
        _meshRenderer.material.SetFloat("_Boiling_Speed", Mathf.Lerp(_maxBoilingSpeed / 2f, _maxBoilingSpeed, boilAmount));
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
            }
        }
    }
}
