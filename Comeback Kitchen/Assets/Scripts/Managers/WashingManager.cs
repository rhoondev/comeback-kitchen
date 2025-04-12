using System.Collections.Generic;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Instruction introductionInstruction;
    [SerializeField] private Instruction washingSectionInstruction;
    [SerializeField] private Instruction firstTurnOnFaucetInstruction;
    [SerializeField] private Instruction washTomatoInstruction;
    [SerializeField] private Instruction washBellPepperInstruction;
    [SerializeField] private Instruction firstTurnOffFaucetInstruction;
    [SerializeField] private Instruction grabOnionInstruction;
    [SerializeField] private Instruction secondTurnOnFaucetInstruction;
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
            cookbook.Close();
        }
        else if (instruction == washTomatoInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == washBellPepperInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == firstTurnOffFaucetInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == grabOnionInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == secondTurnOnFaucetInstruction)
        {
            cookbook.Close();
        }
        else if (instruction == secondTurnOffFaucetInstruction)
        {
            CompleteSection();
        }
    }
}
