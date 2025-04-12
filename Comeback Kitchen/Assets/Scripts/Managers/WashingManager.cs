using System.Collections.Generic;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Instruction introductionInstruction;
    [SerializeField] private Instruction washingSectionInstruction;
    [SerializeField] private Instruction turnOnFaucetInstruction;

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
            cookbook.SetInstruction(turnOnFaucetInstruction);
        }
        else if (instruction == turnOnFaucetInstruction)
        {
            cookbook.Close();
        }
    }
}
