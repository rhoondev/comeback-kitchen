using UnityEngine;

public abstract class SectionManager : MonoBehaviour
{
    [SerializeField] protected Cookbook cookbook;
    [SerializeField] private VRPlayerMover vrPlayerMover;
    [SerializeField] private Transform playerLocation;
    [SerializeField] private Transform cookbookOpenLocation;
    [SerializeField] private Transform cookbookClosedLocation;

    public SmartAction OnSectionStarted = new SmartAction();
    public SmartAction OnSectionCompleted = new SmartAction();

    public virtual void StartSection()
    {
        vrPlayerMover.SetPlayerPosition(playerLocation.position, playerLocation.forward);
        cookbook.SetLocations(cookbookOpenLocation, cookbookClosedLocation);
        cookbook.OnConfirmInstruction.Add(OnConfirmInstruction);
        OnSectionStarted.Invoke();
    }

    protected virtual void CompleteSection()
    {
        cookbook.OnConfirmInstruction.Clear();
        OnSectionCompleted.Invoke();
    }

    protected abstract void OnConfirmInstruction(Instruction instruction);
}
