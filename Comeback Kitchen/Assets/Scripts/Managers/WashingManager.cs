using System;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private Strainer strainer;
    [SerializeField] private CuttingBoard cuttingBoard;
    [SerializeField] private GameObject mussels;
    [SerializeField] private MusselsPlacementZone musselsPlacementZone;
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
            vegetableBasket.SetTargetVegetable("Tomato");
            strainer.OnWashedObjectAdded.Add(OnTomatoAddedToStrainer);
            cookbook.Close();
        }
        else if (instruction == washBellPepperInstruction)
        {
            vegetableBasket.SetTargetVegetable("Bell Pepper");
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
            vegetableBasket.SetTargetVegetable("Onion");
            cuttingBoard.OnVegetableAdded.Add(OnOnionAddedToCuttingBoard);
            cookbook.Close();
        }
        else if (instruction == secondTurnOnFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOn.Add(OnFaucetTurnedOnSecondTime);
            cookbook.Close();
        }
        else if (instruction == washMusselsInstruction)
        {
            musselsPlacementZone.OnMusselsPlacedOnCounter.Add(OnMusselsPlacedOnCounter);
            cookbook.Close();
        }
        else if (instruction == secondTurnOffFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOff.Add(CompleteSection);
            cookbook.Close();
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

    private void OnOnionAddedToCuttingBoard()
    {
        cuttingBoard.OnVegetableAdded.Clear();
        vegetableBasket.gameObject.SetActive(false);
        mussels.SetActive(true);
        cookbook.SetInstruction(secondTurnOnFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOnSecondTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();
        cookbook.SetInstruction(washMusselsInstruction);
        cookbook.Open();
    }

    private void OnMusselsPlacedOnCounter()
    {
        musselsPlacementZone.OnMusselsPlacedOnCounter.Clear();
        cookbook.SetInstruction(secondTurnOffFaucetInstruction);
        cookbook.Open();
    }
}
