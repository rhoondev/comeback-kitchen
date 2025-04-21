using UnityEngine;

public class CutObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Pan>(out var pan))
        {
            pan.Contents.Add(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Pan>(out var pan))
        {
            pan.Contents.Remove(gameObject);
        }
    }
}
