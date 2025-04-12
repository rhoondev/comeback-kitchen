using System;
using UnityEngine;

public abstract class SectionManager : MonoBehaviour
{
    [SerializeField] protected Cookbook cookbook;
    [SerializeField] private Transform playerLocation;
    [SerializeField] private Transform cookbookOpenLocation;
    [SerializeField] private Transform cookbookClosedLocation;

    public event Action OnSectionStarted;
    public event Action OnSectionCompleted;

    public virtual void StartSection()
    {
        // [Move the player to the specified location]
        cookbook.SetLocations(cookbookOpenLocation, cookbookClosedLocation);
        cookbook.OnConfirmInstruction += OnConfirmInstruction;
        OnSectionStarted?.Invoke();
    }

    protected virtual void CompleteSection()
    {
        OnSectionCompleted?.Invoke();
    }

    protected abstract void OnConfirmInstruction(string instructionText);
}
