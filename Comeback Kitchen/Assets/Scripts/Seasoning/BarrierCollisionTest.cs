using UnityEngine;

public class BarrierCollisionTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name}: collision detected with {collision.collider.gameObject.name}");
    }
}
