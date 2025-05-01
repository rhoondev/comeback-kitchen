using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private InteractionLocker faucetInteractionLocker;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private DynamicContainer vegetableStrainer;
    [SerializeField] private DynamicContainer cuttingBoard;
    [SerializeField] private DynamicObject musselStrainer;
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

    // Make sure only one instance of each vegetable exists at a time, to keep things from breaking
    private DynamicObject _tomatoInstance;
    private DynamicObject _bellPepperInstance;
    private DynamicObject _onionInstance;

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
            faucetInteractionLocker.UnlockInteraction();
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
            faucetInteractionLocker.UnlockInteraction();
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
            faucetInteractionLocker.UnlockInteraction();
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
            faucetInteractionLocker.UnlockInteraction();
            faucet.OnTurnedFullyOff.Add(OnFaucetTurnedOffSecondTime);
            cookbook.Close();
        }
    }

    private void OnFaucetTurnedOnFirstTime()
    {
        faucetInteractionLocker.LockInteraction();
        faucet.OnTurnedFullyOn.Clear();
        cookbook.SetInstruction(washTomatoInstruction);
        cookbook.Open();
    }

    private void OnTomatoGrabbed(DynamicObject tomato)
    {
        if (_tomatoInstance != null)
        {
            _tomatoInstance.GetComponent<Washable>().OnWashed.Clear();
            Destroy(_tomatoInstance.gameObject);
        }

        _tomatoInstance = tomato;

        // Disable the vegetable strainer in case the player tries to wash one object and then put another unwashed object in the strainer
        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.OnObjectAdded.Clear();

        tomato.GetComponent<Washable>().OnWashed.Add(OnTomatoWashed);
    }

    private void OnTomatoWashed(Washable tomato)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(tomato.GetComponent<DynamicObject>());
        vegetableStrainer.OnObjectAdded.Add(OnTomatoAddedToStrainer);
    }

    private void OnTomatoAddedToStrainer(DynamicObject tomato)
    {
        tomato.GetComponent<Washable>().HideProgressBar();

        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.OnObjectAdded.Clear();

        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);

        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperGrabbed(DynamicObject bellPepper)
    {
        if (_bellPepperInstance != null)
        {
            _bellPepperInstance.GetComponent<Washable>().OnWashed.Clear();
            Destroy(_bellPepperInstance.gameObject);
        }

        _bellPepperInstance = bellPepper;

        // Disable the vegetable strainer in case the player tries to wash one object and then put another unwashed object in the strainer
        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.OnObjectAdded.Clear();

        bellPepper.GetComponent<Washable>().OnWashed.Add(OnBellPepperWashed);
    }

    private void OnBellPepperWashed(Washable bellpepper)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(bellpepper.GetComponent<DynamicObject>());
        vegetableStrainer.OnObjectAdded.Add(OnBellPepperAddedToStrainer);
    }

    private void OnBellPepperAddedToStrainer(DynamicObject bellPepper)
    {
        bellPepper.GetComponent<Washable>().HideProgressBar();

        vegetableStrainer.OnObjectAdded.Clear();
        vegetableStrainer.SetTargetObject(null);
        vegetableStrainer.DisableReceivingObjects();

        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);

        cookbook.SetInstruction(firstTurnOffFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOffFirstTime()
    {
        faucetInteractionLocker.LockInteraction();
        faucet.OnTurnedFullyOff.Clear();
        cookbook.SetInstruction(grabOnionInstruction);
        cookbook.Open();
    }

    private void OnOnionGrabbed(DynamicObject onion)
    {
        if (_onionInstance != null)
        {
            Destroy(_onionInstance.gameObject);
        }

        _onionInstance = onion;

        cuttingBoard.EnableReceivingObjects();
        cuttingBoard.SetTargetObject(onion);
        cuttingBoard.OnObjectAdded.Add(OnOnionAddedToCuttingBoard);
    }

    private void OnOnionAddedToCuttingBoard(DynamicObject _)
    {
        cuttingBoard.OnObjectAdded.Clear();
        cuttingBoard.SetTargetObject(null);
        cuttingBoard.DisableReceivingObjects();

        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);
        vegetableBasket.gameObject.SetActive(false);

        musselStrainer.gameObject.SetActive(true);

        cookbook.SetInstruction(secondTurnOnFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOnSecondTime()
    {
        faucetInteractionLocker.LockInteraction();
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

    private void OnFaucetTurnedOffSecondTime()
    {
        faucetInteractionLocker.LockInteraction();
        faucet.OnTurnedFullyOff.Clear();

        CompleteSection();
    }
}
