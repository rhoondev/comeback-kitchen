using System;
using UnityEngine;

public enum LiquidType
{
    Oil,
    Water,
    TomatoJuice
}

public class Liquid : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [field: SerializeField] public LiquidType Type { get; private set; }
    [field: SerializeField] public int MaxFillCount { get; private set; }
    [field: SerializeField] public int FillCount { get; private set; }

    public bool IsFull { get => FillCount == MaxFillCount; }
    public bool IsEmpty { get => FillCount == 0; }

    public float MaxFillHeight { get; private set; }
    public float FillCutoff { get; private set; }

    public int Fill(int amount)
    {
        int amountToAdd = Math.Min(amount, MaxFillCount - FillCount);
        FillCount += amountToAdd;
        return amountToAdd;
    }

    public int Drain(int amount)
    {
        int amountToSubtract = Math.Min(amount, FillCount);
        FillCount -= amountToSubtract;
        return amountToSubtract;
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
        float maxY = meshRenderer.bounds.max.y - 0.001f;

        FillCutoff = Mathf.Lerp(minY, maxY, fillAmount);

        meshRenderer.material.SetFloat("_Fill_Height", FillCutoff);
    }
}
