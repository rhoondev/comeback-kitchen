using UnityEngine;

public class RiceFiller : MonoBehaviour
{
    [SerializeField] private Container container;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Rice _))
        {
            GameObject rice = container.RestoreObject();

            if (TryGetComponent(out Pan pan))
            {
                pan.Contents.Add(rice);
            }

            Destroy(other.gameObject);
        }
    }
}