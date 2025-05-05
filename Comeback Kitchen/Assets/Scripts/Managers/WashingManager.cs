using UnityEngine;

public class WashingManager : SectionManager
{
    [SerializeField] private Faucet faucet;
    [SerializeField] private VegetableBasket vegetableBasket;
    [SerializeField] private DynamicContainer vegetableStrainer;
    [SerializeField] private DynamicObject musselStrainer;
    [SerializeField] private Washable musselsWashable;
    [SerializeField] private DynamicContainer musselStrainerZone;
    [SerializeField] private DynamicContainer cuttingBoardZone;

    [SerializeField] private Instruction introductionInstruction;
    [SerializeField] private Instruction washingSectionInstruction;
    [SerializeField] private Instruction turnOnFaucetInstruction;
    [SerializeField] private Instruction washTomatoInstruction;
    [SerializeField] private Instruction washBellPepperInstruction;
    [SerializeField] private Instruction washMusselsInstruction;
    [SerializeField] private Instruction turnOffFaucetInstruction;
    [SerializeField] private Instruction grabOnionInstruction;
    [SerializeField] private Instruction sectionCompletedInstruction;

    // Make sure only one instance of each vegetable exists at a time, to keep things from breaking
    private DynamicObject _tomatoInstance;
    private DynamicObject _bellPepperInstance;
    private DynamicObject _onionInstance;

    public override void StartSection()
    {
        base.StartSection();
        cookbook.SetInstruction(introductionInstruction);
        cookbook.ChangeInstructionConfirmationText("Comenzar");
    }

    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (instruction == introductionInstruction)
        {
            cookbook.SetInstruction(washingSectionInstruction);
        }
        else if (instruction == washingSectionInstruction)
        {
            cookbook.SetInstruction(turnOnFaucetInstruction);
            cookbook.ChangeInstructionConfirmationText("Continuar");
        }
        else if (instruction == turnOnFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOn.Add(OnFaucetTurnedOn);

            cookbook.Close();
        }
        else if (instruction == washTomatoInstruction)
        {
            vegetableBasket.Unlock();
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
        else if (instruction == washMusselsInstruction)
        {
            musselStrainerZone.OnObjectReceived.Add(OnMusselsPlacedInSink);

            cookbook.Close();
        }
        else if (instruction == turnOffFaucetInstruction)
        {
            faucet.UnlockLever();
            faucet.OnTurnedFullyOff.Add(OnFaucetTurnedOff);

            cookbook.Close();
        }
        else if (instruction == grabOnionInstruction)
        {
            vegetableBasket.Unlock();
            vegetableBasket.SetActiveVegetableType(VegetableType.Onion);
            vegetableBasket.OnVegetableGrabbed.Add(OnOnionGrabbed);

            cookbook.Close();
        }
        else if (instruction == sectionCompletedInstruction)
        {
            CompleteSection();
        }
    }

    private void OnFaucetTurnedOn()
    {
        faucet.LockLever();
        faucet.OnTurnedFullyOn.Clear();

        cookbook.SetInstruction(washTomatoInstruction);
        cookbook.Open();
    }

    private void OnTomatoGrabbed(DynamicObject tomato)
    {
        if (_tomatoInstance != null)
        {
            Destroy(_tomatoInstance.gameObject);
        }

        _tomatoInstance = tomato;

        // Turn off the vegetable strainer receiving objects in case the player washes one object and then puts another unwashed object in the strainer
        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.OnObjectReceived.Clear();

        tomato.GetComponent<Washable>().OnWashed.Add(OnTomatoWashed);
    }

    private void OnTomatoWashed(Washable tomato)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(tomato.GetComponent<DynamicObject>());
        vegetableStrainer.OnObjectReceived.Add(OnTomatoAddedToStrainer);
    }

    private void OnTomatoAddedToStrainer(DynamicObject tomato)
    {
        tomato.GetComponent<Washable>().HideProgressBar();

        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.OnObjectReceived.Clear();

        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);

        cookbook.SetInstruction(washBellPepperInstruction);
        cookbook.Open();
    }

    private void OnBellPepperGrabbed(DynamicObject bellPepper)
    {
        if (_bellPepperInstance != null)
        {
            Destroy(_bellPepperInstance.gameObject);
        }

        _bellPepperInstance = bellPepper;

        // Turn off the vegetable strainer receiving objects in case the player washes one object and then puts another unwashed object in the strainer
        vegetableStrainer.DisableReceivingObjects();
        vegetableStrainer.OnObjectReceived.Clear();

        bellPepper.GetComponent<Washable>().OnWashed.Add(OnBellPepperWashed);
    }

    private void OnBellPepperWashed(Washable bellPepper)
    {
        vegetableStrainer.EnableReceivingObjects();
        vegetableStrainer.SetTargetObject(bellPepper.GetComponent<DynamicObject>());
        vegetableStrainer.OnObjectReceived.Add(OnBellPepperAddedToStrainer);
    }

    private void OnBellPepperAddedToStrainer(DynamicObject bellPepper)
    {
        bellPepper.GetComponent<Washable>().HideProgressBar();

        vegetableStrainer.OnObjectReceived.Clear();
        vegetableStrainer.DisableReceivingObjects();

        vegetableBasket.Lock();
        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);

        musselStrainer.GetComponent<InteractionLocker>().UnlockInteraction();
        musselsWashable.OnWashed.Add(OnMusselsWashed);

        cookbook.SetInstruction(washMusselsInstruction);
        cookbook.Open();
    }

    private void OnMusselsWashed(Washable _)
    {
        musselsWashable.OnWashed.Clear();

        // The mussel strainer already belongs to the mussel strainer zone, so we have to use OnObjectReReceived instead of OnObjectReceived
        musselStrainerZone.EnableReReceivingMode();
        musselStrainerZone.OnObjectReReceived.Add(OnMusselsPlacedInSink);
    }

    private void OnMusselsPlacedInSink(DynamicObject _)
    {
        musselsWashable.HideProgressBar();

        musselStrainerZone.DisableReceivingObjects();
        musselStrainerZone.OnObjectReReceived.Clear();

        cookbook.SetInstruction(turnOffFaucetInstruction);
        cookbook.Open();
    }

    private void OnFaucetTurnedOff()
    {
        faucet.LockLever();
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

        cuttingBoardZone.EnableReceivingObjects();
        cuttingBoardZone.SetTargetObject(onion);
        cuttingBoardZone.OnObjectReceived.Add(OnOnionAddedToCuttingBoard);
    }

    private void OnOnionAddedToCuttingBoard(DynamicObject _)
    {
        cuttingBoardZone.OnObjectReceived.Clear();
        cuttingBoardZone.DisableReceivingObjects();

        vegetableBasket.Lock();
        vegetableBasket.OnVegetableGrabbed.Clear();
        vegetableBasket.SetActiveVegetableType(null);

        cookbook.SetInstruction(sectionCompletedInstruction);
        cookbook.Open();
    }
}
