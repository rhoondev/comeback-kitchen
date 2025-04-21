using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private Flame activeFlame;
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private float maxTemperature;
    [SerializeField] private float thermalConductivity;
    [SerializeField] private float localStirRadius;
    [SerializeField] private float localStirStrength;
    [SerializeField] private float globalStirStrength;
    [SerializeField] private float globalStirAmount;

    public List<GameObject> Contents { get; private set; }

    private void Awake()
    {
        Contents = new List<GameObject>();
    }

    public void ApplyStir(Vector3 position, Vector3 velocity)
    {
        Vector2 spoonPos = new Vector2(position.x, position.z);
        Vector2 spoonVel = new Vector2(velocity.x, velocity.z);

        if (globalStirAmount > 0f)
        {
            Vector2 center = new Vector2(transform.position.x, transform.position.z);
            Vector2 offset = center - spoonPos;
            Vector2 perpendicular = new Vector2(-offset.y, offset.x);
            float stirSpeed = Vector2.Dot(perpendicular.normalized, spoonVel) * globalStirStrength * globalStirAmount;

            foreach (var item in Contents)
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
        else
        {
            foreach (var item in Contents)
            {
                if (item.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }

        foreach (var item in Contents)
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
                    rb.linearVelocity += new Vector3(spoonVel.x, 0f, spoonVel.y) * stirSpeed;
                }
            }
        }
    }
}
