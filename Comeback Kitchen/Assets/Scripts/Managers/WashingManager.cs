using System;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private PlacementZone strainerPlacementZone;
    [SerializeField] private PlacementZone cuttingBoardPlacementZone;
    [SerializeField] private GameObject musselsStrainer;
    [SerializeField] private Washable mussels;
    [SerializeField] private PlacementZone musselsPlacementZone;

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
            vegetableBasket.OnVegetableGrabbed.Add(OnTomatoGrabbed);
            cookbook.Close();
        }
        else if (instruction == washBellPepperInstruction)
        {
            vegetableBasket.SetTargetVegetable("Bell Pepper");
            vegetableBasket.OnVegetableGrabbed.Add(OnBellPepperGrabbed);
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
            vegetableBasket.OnVegetableGrabbed.Add(OnOnionGrabbed);
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
            musselsPlacementZone.OnObjectEnter.Add(OnMusselsPlacedOnCounter);
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

    private void OnTomatoGrabbed(GameObject tomato)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        tomato.GetComponent<Washable>().OnWashed.Add(OnTomatoWashed);
    }

    private void OnTomatoWashed()
    {
        strainerPlacementZone.gameObject.SetActive(true);
        strainerPlacementZone.OnObjectEnter.Add(OnTomatoAddedToStrainer);
    }

    private void OnTomatoAddedToStrainer()
    {
        strainerPlacementZone.OnObjectEnter.Clear();
        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperGrabbed(GameObject bellPepper)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        bellPepper.GetComponent<Washable>().OnWashed.Add(OnBellPepperWashed);
    }
    private void OnBellPepperWashed()
    {
        strainerPlacementZone.gameObject.SetActive(true);
        strainerPlacementZone.OnObjectEnter.Add(OnBellPepperAddedToStrainer);
    }

    private void OnBellPepperAddedToStrainer()
    {
        strainerPlacementZone.OnObjectEnter.Clear();
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

    private void OnOnionGrabbed(GameObject onion)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        cuttingBoardPlacementZone.gameObject.SetActive(true);
        cuttingBoardPlacementZone.OnObjectEnter.Add(OnOnionAddedToCuttingBoard);
    }

    private void OnOnionAddedToCuttingBoard()
    {
        cuttingBoardPlacementZone.OnObjectEnter.Clear();
        vegetableBasket.gameObject.SetActive(false);
        musselsStrainer.SetActive(true);
        cookbook.SetInstruction(secondTurnOnFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOnSecondTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();
        mussels.OnWashed.Add(OnMusselsWashed);
        cookbook.SetInstruction(washMusselsInstruction);
        cookbook.Open();
    }

    private void OnMusselsWashed()
    {
        mussels.OnWashed.Clear();
        musselsPlacementZone.gameObject.SetActive(true);
        musselsPlacementZone.OnObjectEnter.Add(OnMusselsPlacedOnCounter);
    }

    private void OnMusselsPlacedOnCounter()
    {
        musselsPlacementZone.OnObjectEnter.Clear();
        cookbook.SetInstruction(secondTurnOffFaucetInstruction);
        cookbook.Open();
    }
}
