using UnityEngine;

public class Fillable : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Renderer liquidRenderer;

    public bool IsFull { get => _fillCount >= MAX_FILL_COUNT; }

    private const int MAX_FILL_COUNT = 100;

    private int _fillCount = 0;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Stream"))
        {
            if (!IsFull)
            {
                _fillCount++;

                float fillAmount = (float)_fillCount / MAX_FILL_COUNT;
                float fillHeight = liquidRenderer.material.GetFloat("_MaxHeight") * fillAmount;

                boxCollider.size = new Vector3(boxCollider.size.x, fillHeight, boxCollider.size.z);
                boxCollider.center = new Vector3(boxCollider.center.x, fillHeight / 2f, boxCollider.center.z);
                liquidRenderer.material.SetFloat("_FillAmount", fillAmount);
            }
        }
    }
}
