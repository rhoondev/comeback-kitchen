using UnityEngine;

public class Washable : MonoBehaviour
{
    public bool IsWashed { get => _amountWashed >= WASH_QUOTA; }

    private const int WASH_QUOTA = 100;

    private int _amountWashed = 0;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Stream"))
        {
            bool wasWashed = IsWashed;

            _amountWashed++;

            if (IsWashed && !wasWashed)
            {
                Debug.Log($"Finished washing {gameObject.name}");
            }
        }
    }
}
