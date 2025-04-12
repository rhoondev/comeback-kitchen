using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private Strainer strainer;
    [SerializeField] private Instruction introductionInstruction;
    [SerializeField] private Instruction washingSectionInstruction;
    [SerializeField] private Instruction firstTurnOnFaucetInstruction;
    [SerializeField] private Instruction washTomatoInstruction;
    [SerializeField] private Instruction washBellPepperInstruction;
    [SerializeField] private Instruction firstTurnOffFaucetInstruction;
    [SerializeField] private Instruction grabOnionInstruction;
    [SerializeField] private Instruction secondTurnOnFaucetInstruction;
    [SerializeField] private Instruction washMusselsInstruction;
    [SerializeField] private Instruction secondTurnOffFaucetInstruction;

    public override void StartSection()
    {
        base.StartSection();
        cookbook.SetInstruction(introductionInstruction);
        cookbook.ChangeInstructionConfirmationText("Start");
    }

    protected override void CompleteSection()
    {
        base.CompleteSection();
    }

    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (instruction == introductionInstruction)
        {
            cookbook.SetInstruction(washingSectionInstruction);
            cookbook.ChangeInstructionConfirmationText("Continue");
        }
        else if (instruction == washingSectionInstruction)
        {
            cookbook.SetInstruction(firstTurnOnFaucetInstruction);
        }
        else if (instruction == firstTurnOnFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOn.Add(OnFaucetTurnedOnFirstTime);
            cookbook.Close();
        }
        else if (instruction == washTomatoInstruction)
        {
            strainer.OnWashedObjectAdded.Add(OnTomatoAddedToStrainer);
            cookbook.Close();
        }
        else if (instruction == washBellPepperInstruction)
        {
            strainer.OnWashedObjectAdded.Add(OnBellPepperAddedToStrainer);
            cookbook.Close();
        }
        else if (instruction == firstTurnOffFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOff.Add(OnFaucetTurnedOffFirstTime);
            cookbook.Close();
        }
        else if (instruction == grabOnionInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == secondTurnOnFaucetInstruction)
        {
            faucet.UnlockLever();
            cookbook.Close();
        }
        else if (instruction == washMusselsInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == secondTurnOffFaucetInstruction)
        {
            faucet.UnlockLever();
        }
    }

    private void OnFaucetTurnedOnFirstTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();
        cookbook.SetInstruction(washTomatoInstruction);
        cookbook.Open();
    }

    private void OnTomatoAddedToStrainer()
    {
        strainer.OnWashedObjectAdded.Clear();
        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperAddedToStrainer()
    {
        strainer.OnWashedObjectAdded.Clear();
        cookbook.SetInstruction(firstTurnOffFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOffFirstTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOff.Clear();
        cookbook.SetInstruction(grabOnionInstruction);
        cookbook.Open();
    }
}
