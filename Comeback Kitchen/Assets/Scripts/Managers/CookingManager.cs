using UnityEngine;

public class CookingManager : SectionManager
{
    [SerializeField] private Stove stove;
    [SerializeField] private PlacementZone panPlacementZone;

    [SerializeField] private Instruction cookingSectionInstruction;
    [SerializeField] private Instruction turnStoveToMediumHighInstruction;
    [SerializeField] private Instruction slidePanInstruction;
    [SerializeField] private Instruction pourOliveOilInstruction;

    public override void StartSection()
    {
        base.StartSection();
        cookbook.SetInstruction(cookingSectionInstruction);
        cookbook.Open();
    }

    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (instruction == cookingSectionInstruction)
        {
            cookbook.SetInstruction(turnStoveToMediumHighInstruction);
        }
        else if (instruction == turnStoveToMediumHighInstruction)
        {
            stove.UnlockKnob();
            stove.OnSettingChanged.Add(OnStoveTurnedOnFirstTime);
            cookbook.Close();
        }
        else if (instruction == slidePanInstruction)
        {
            panPlacementZone.gameObject.SetActive(true);
            panPlacementZone.OnObjectEnter.Add(OnPanPlacedOnStove);
            cookbook.Close();
        }
        else if (instruction == pourOliveOilInstruction)
        {
            // Logic for pouring olive oil
            cookbook.Close();
        }
    }

    private void OnStoveTurnedOnFirstTime(StoveSetting setting)
    {
        if (setting == StoveSetting.MediumHigh)
        {
            stove.LockKnob();
            stove.OnSettingChanged.Clear();
            cookbook.SetInstruction(slidePanInstruction);
            cookbook.Open();
        }
    }

    private void OnPanPlacedOnStove()
    {
        panPlacementZone.OnObjectEnter.Clear();
        panPlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(pourOliveOilInstruction);
        cookbook.Open();
    }
}
