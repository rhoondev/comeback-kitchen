using UnityEngine;

public class RiceFiller : MonoBehaviour
{
    [SerializeField] private Container container;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Rice _))
        {
            container.RestoreObject();
            Destroy(other.gameObject);
        }
    }
}