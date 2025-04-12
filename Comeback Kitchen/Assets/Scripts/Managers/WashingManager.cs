using System.Collections.Generic;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private string introductionText;
    [SerializeField] private List<Sprite> introductionImages;
    [SerializeField] private string washingSectionText;
    [SerializeField] private List<Sprite> washingSectionImages;
    [SerializeField] private string turnOnFaucetText;
    [SerializeField] private List<Sprite> turnOnFaucetImages;

    public override void StartSection()
    {
        base.StartSection();
        cookbook.SetInstruction(introductionText, introductionImages);
        cookbook.ChangeInstructionConfirmationText("Start");
    }

    protected override void CompleteSection()
    {
        base.CompleteSection();
    }

    protected override void OnConfirmInstruction(string instructionText)
    {
        if (instructionText == introductionText)
        {
            cookbook.SetInstruction(washingSectionText, washingSectionImages);
            cookbook.ChangeInstructionConfirmationText("Continue");
        }
        else if (instructionText == washingSectionText)
        {
            cookbook.SetInstruction(turnOnFaucetText, turnOnFaucetImages);
        }
        else if (instructionText == turnOnFaucetText)
        {
            cookbook.Close();
        }
    }
}
