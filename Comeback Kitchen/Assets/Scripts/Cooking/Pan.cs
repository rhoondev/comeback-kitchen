using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    enum StirringMode
    {
        Local,
        Global
    }

    [SerializeField] private Flame activeFlame;
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private float maxTemperature;
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float localStirRadius;
    [SerializeField] private float localStirStrength;
    [SerializeField] private float globalStirStrength;
    [SerializeField] private StirringMode stirringMode;

    private float _temperature = 70f;
    private List<Cookable> _contents;

    private void Awake()
    {
        _contents = new List<Cookable>();
    }

    private void Update()
    {
        float ambient = maxTemperature * activeFlame.Size;
        float rate = thermalConductivity * (ambient - _temperature);
        _temperature += rate * Time.deltaTime;

        foreach (var item in _contents)
        {
            item.Cook(_temperature);
        }

        panLiquid.Heat(_temperature);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            _contents.Add(cookable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            _contents.Remove(cookable);
        }
    }

    public void ApplyStir(Vector3 position, Vector3 velocity)
    {
        if (stirringMode == StirringMode.Local)
        {
            Vector2 spoonPos = new Vector2(position.x, position.z);
            Vector2 spoonVel = new Vector2(velocity.x, velocity.z);

            foreach (var item in _contents)
            {
                if (item.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector2 itemPos = new Vector2(item.transform.position.x, item.transform.position.z);
                    Vector2 spoonToItem = itemPos - spoonPos;
                    float distance = spoonToItem.magnitude;
                    float dot = Vector2.Dot(spoonToItem.normalized, spoonVel.normalized);

                    if (dot > 0f && distance < localStirRadius)
                    {
                        float stirSpeed = Mathf.Clamp01(localStirStrength * dot * (1f - distance / localStirRadius));
                        rb.linearVelocity = new Vector3(spoonVel.x, 0f, spoonVel.y) * stirSpeed;
                    }
                }
            }
        }
        else if (stirringMode == StirringMode.Global)
        {
            Vector2 center = new Vector2(transform.position.x, transform.position.z);
            Vector2 spoonPos = new Vector2(position.x, position.z);
            Vector2 spoonVel = new Vector2(velocity.x, velocity.z);
            Vector2 offset = center - spoonPos;
            Vector2 perpendicular = new Vector2(-offset.y, offset.x);
            float stirSpeed = Vector2.Dot(perpendicular.normalized, spoonVel) * globalStirStrength;

            foreach (var item in _contents)
            {
                if (item.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector2 itemPos = new Vector2(item.transform.position.x, item.transform.position.z);
                    Vector2 itemOffset = center - itemPos;
                    Vector2 moveDir = new Vector2(-itemOffset.y, itemOffset.x).normalized;
                    rb.linearVelocity = new Vector3(moveDir.x, 0f, moveDir.y) * stirSpeed;
                }
            }
        }
    }
}
