using UnityEngine;

public class Liquid : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [field: SerializeField] public int MaxFillCount { get; private set; }
    [field: SerializeField] public int FillCount { get; private set; }

    public bool IsFull { get => FillCount == MaxFillCount; }
    public bool IsEmpty { get => FillCount == 0; }
    public float MaxFillHeight { get; private set; }

    public void Fill(int amount)
    {
        if (!IsFull)
        {
            FillCount += amount;
            UpdateMaterialProperties();
        }
    }

    public void Drain(int amount)
    {
        if (!IsEmpty)
        {
            FillCount -= amount;
            UpdateMaterialProperties();
        }
    }

    private void Awake()
    {
        MeshFilter meshFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
        MaxFillHeight = meshFilter.mesh.bounds.size.y;
        meshRenderer.material.SetFloat("_MaxHeight", MaxFillHeight);

        UpdateMaterialProperties();
    }

    private void UpdateMaterialProperties()
    {
        float fillAmount = (float)FillCount / MaxFillCount;
        meshRenderer.material.SetFloat("_FillAmount", fillAmount);
    }
}
