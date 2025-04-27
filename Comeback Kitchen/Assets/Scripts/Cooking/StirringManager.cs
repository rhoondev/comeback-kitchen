using UnityEngine;

public class StirringManager : MonoBehaviour
{
    [SerializeField] private Container panContainer;
    [SerializeField] private float localStirRadius;
    [SerializeField] private float localStirStrength;
    [SerializeField] private float globalStirStrength;
    [SerializeField] private float globalStirAmount;

    // Must be called during FixedUpdate
    public void ApplyStir(Vector3 position, Vector3 velocity)
    {
        Vector2 spoonPos = new Vector2(position.x, position.z);
        Vector2 spoonVel = new Vector2(velocity.x, velocity.z);
        Vector2 center = new Vector2(transform.position.x, transform.position.z);

        Vector2 spoonToCenter = center - spoonPos;
        Vector2 perpendicular = new Vector2(-spoonToCenter.y, spoonToCenter.x);
        Vector2 normalizedPerpendicular = perpendicular.normalized;
        Vector2 normalizedSpoonVel = spoonVel.normalized;

        float globalStirSpeed = Vector2.Dot(normalizedPerpendicular, spoonVel) * globalStirStrength * globalStirAmount;
        float sqrRadius = localStirRadius * localStirRadius;

        foreach (var obj in panContainer.Objects)
        {
            Vector3 itemWorldPos = obj.transform.position;
            Vector2 itemPos = new Vector2(itemWorldPos.x, itemWorldPos.z);
            Vector2 itemToCenter = center - itemPos;
            Vector2 moveDir = new Vector2(-itemToCenter.y, itemToCenter.x).normalized;
            Vector3 stirVelocity = new Vector3(moveDir.x, 0f, moveDir.y) * globalStirSpeed;

            Vector2 spoonToItem = itemPos - spoonPos;
            float sqrDistance = spoonToItem.sqrMagnitude;
            float dot = Vector2.Dot(spoonToItem.normalized, normalizedSpoonVel);

            if (dot > 0f && sqrDistance < sqrRadius)
            {
                // Use approximate falloff to avoid sqrt
                float distanceRatio = 1f - sqrDistance / sqrRadius;
                float stirSpeed = Mathf.Clamp01(localStirStrength * dot * distanceRatio);
                stirVelocity += new Vector3(spoonVel.x, 0f, spoonVel.y) * stirSpeed;
            }

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.MovePosition(rb.position + stirVelocity * Time.fixedDeltaTime);
        }
    }
}
