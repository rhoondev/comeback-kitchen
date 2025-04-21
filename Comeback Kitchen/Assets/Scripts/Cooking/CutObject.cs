using UnityEngine;

public class CutObject : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Pan>(out var pan))
        {
            pan.Contents.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Pan>(out var pan))
        {
            pan.Contents.Remove(rb);
        }
    }
}
