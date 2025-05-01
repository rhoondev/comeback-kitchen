using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private DynamicContainer vegetableStrainer;
    [SerializeField] private DynamicContainer cuttingBoard;
    [SerializeField] private GameObject musselStrainer;
    [SerializeField] private Washable musselsWashableTarget;
    [SerializeField] private DynamicContainer musselStrainerPlacementZone;

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
        }
        else if (instruction == washingSectionInstruction)
        {
            cookbook.SetInstruction(firstTurnOnFaucetInstruction);
            cookbook.ChangeInstructionConfirmationText("Continue");
        }
        else if (instruction == firstTurnOnFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOn.Add(OnFaucetTurnedOnFirstTime);
            cookbook.Close();
        }
        else if (instruction == washTomatoInstruction)
        {
            vegetableBasket.SetActiveVegetableType(VegetableType.Tomato);
            vegetableBasket.OnVegetableGrabbed.Add(OnTomatoGrabbed);
            cookbook.Close();
        }
        else if (instruction == washBellPepperInstruction)
        {
            vegetableBasket.SetActiveVegetableType(VegetableType.BellPepper);
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
            vegetableBasket.SetActiveVegetableType(VegetableType.Onion);
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
            musselStrainerPlacementZone.OnObjectAdded.Add(OnMusselsPlacedOnCounter);
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

    private void OnTomatoWashed(Washable tomato)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(tomato.gameObject);
        vegetableStrainer.OnObjectAdded.Add(OnTomatoAddedToStrainer);
    }

    private void OnTomatoAddedToStrainer(DynamicObject tomato)
    {
        tomato.GetComponent<Washable>().HideProgressBar();
        vegetableStrainer.OnObjectAdded.Clear();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.DisableReceivingObjects();
        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperGrabbed(GameObject bellPepper)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        bellPepper.GetComponent<Washable>().OnWashed.Add(OnBellPepperWashed);
    }

    private void OnBellPepperWashed(Washable bellPepper)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(bellPepper.gameObject);
        vegetableStrainer.OnObjectAdded.Add(OnBellPepperAddedToStrainer);
    }

    private void OnBellPepperAddedToStrainer(DynamicObject bellPepper)
    {
        bellPepper.GetComponent<Washable>().HideProgressBar();
        vegetableStrainer.OnObjectAdded.Clear();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.DisableReceivingObjects();
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

        cuttingBoard.EnableReceivingObjects();
        cuttingBoard.SetTargetObject(onion);
        cuttingBoard.OnObjectAdded.Add(OnOnionAddedToCuttingBoard);
    }

    private void OnOnionAddedToCuttingBoard(DynamicObject _)
    {
        cuttingBoard.OnObjectAdded.Clear();
        cuttingBoard.SetTargetObject(null);
        cuttingBoard.DisableReceivingObjects();

        vegetableBasket.gameObject.SetActive(false);
        musselStrainer.SetActive(true);

        cookbook.SetInstruction(secondTurnOnFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOnSecondTime()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();

        musselStrainer.GetComponent<InteractionLocker>().UnlockInteraction();
        musselsWashableTarget.OnWashed.Add(OnMusselsWashed);

        cookbook.SetInstruction(washMusselsInstruction);
        cookbook.Open();
    }

    private void OnMusselsWashed(Washable _)
    {
        musselsWashableTarget.OnWashed.Clear();
        musselStrainerPlacementZone.EnableReceivingObjects();
        musselStrainerPlacementZone.SetTargetObject(musselStrainer);
        musselStrainerPlacementZone.OnObjectAdded.Add(OnMusselsPlacedOnCounter);
    }

    private void OnMusselsPlacedOnCounter(DynamicObject _)
    {
        musselStrainerPlacementZone.OnObjectAdded.Clear();
        musselStrainerPlacementZone.SetTargetObject(null);
        musselStrainerPlacementZone.DisableReceivingObjects();
        cookbook.SetInstruction(secondTurnOffFaucetInstruction);
        cookbook.Open();
    }
}
