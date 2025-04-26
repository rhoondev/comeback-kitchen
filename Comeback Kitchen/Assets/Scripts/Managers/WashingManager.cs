using System;
using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private PlacementZone strainerPlacementZone;
    [SerializeField] private PlacementZone cuttingBoardPlacementZone;
    [SerializeField] private GameObject musselsStrainer;
    [SerializeField] private Washable musselsWashableTarget;
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

    private GameObject _tomato;
    private GameObject _bellPepper;

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
        _tomato = tomato;
    }

    private void OnTomatoWashed()
    {
        strainerPlacementZone.gameObject.SetActive(true);
        strainerPlacementZone.SetTargetObject(_tomato);
        strainerPlacementZone.OnObjectEnter.Add(OnTomatoAddedToStrainer);
    }

    private void OnTomatoAddedToStrainer()
    {
        strainerPlacementZone.OnObjectEnter.Clear();
        strainerPlacementZone.SetTargetObject(null);
        strainerPlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperGrabbed(GameObject bellPepper)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        bellPepper.GetComponent<Washable>().OnWashed.Add(OnBellPepperWashed);
        _bellPepper = bellPepper;
    }

    private void OnBellPepperWashed()
    {
        strainerPlacementZone.gameObject.SetActive(true);
        strainerPlacementZone.SetTargetObject(_bellPepper);
        strainerPlacementZone.OnObjectEnter.Add(OnBellPepperAddedToStrainer);
    }

    private void OnBellPepperAddedToStrainer()
    {
        strainerPlacementZone.OnObjectEnter.Clear();
        strainerPlacementZone.SetTargetObject(null);
        strainerPlacementZone.gameObject.SetActive(false);
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
        cuttingBoardPlacementZone.SetTargetObject(onion);
        cuttingBoardPlacementZone.OnObjectEnter.Add(OnOnionAddedToCuttingBoard);
    }

    private void OnOnionAddedToCuttingBoard()
    {
        cuttingBoardPlacementZone.OnObjectEnter.Clear();
        cuttingBoardPlacementZone.SetTargetObject(null);
        cuttingBoardPlacementZone.gameObject.SetActive(false);
        vegetableBasket.gameObject.SetActive(false);
        musselsStrainer.SetActive(true);
        cookbook.SetInstruction(secondTurnOnFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOnSecondTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();
        musselsWashableTarget.OnWashed.Add(OnMusselsWashed);
        cookbook.SetInstruction(washMusselsInstruction);
        cookbook.Open();
    }

    private void OnMusselsWashed()
    {
        musselsWashableTarget.OnWashed.Clear();
        musselsPlacementZone.gameObject.SetActive(true);
        musselsPlacementZone.SetTargetObject(musselsStrainer);
        musselsPlacementZone.OnObjectEnter.Add(OnMusselsPlacedOnCounter);
    }

    private void OnMusselsPlacedOnCounter()
    {
        musselsPlacementZone.OnObjectEnter.Clear();
        musselsPlacementZone.SetTargetObject(null);
        musselsPlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(secondTurnOffFaucetInstruction);
        cookbook.Open();
    }
}
