using UnityEngine;

public class Liquid : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [field: SerializeField] public int MaxFillCount { get; private set; }
    [field: SerializeField] public int FillCount { get; private set; }

    public bool IsFull { get => FillCount == MaxFillCount; }
    public bool IsEmpty { get => FillCount == 0; }


    public float MaxFillHeight { get; private set; }
    public float FillCutoff { get; private set; }

    public void Fill(int amount)
    {
        if (!IsFull)
        {
            FillCount += amount;
        }
    }

    public void Drain(int amount)
    {
        if (!IsEmpty)
        {
            FillCount -= amount;
        }
    }

    private void Awake()
    {
        MeshFilter meshFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
        MaxFillHeight = meshFilter.mesh.bounds.size.y;
    }

    private void Update()
    {
        float fillAmount = (float)FillCount / MaxFillCount;
        float minY = meshRenderer.bounds.min.y;
        float maxY = meshRenderer.bounds.max.y;

        FillCutoff = Mathf.Lerp(minY, maxY, fillAmount);

        meshRenderer.material.SetFloat("_FillHeight", FillCutoff);
    }
}
