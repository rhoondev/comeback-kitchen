using UnityEngine;

public class CookingManager : SectionManager
{
    [SerializeField] private Stove stove;
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private DynamicObject panDynamicObject;
    [SerializeField] private DynamicContainer panDynamicContainer;
    [SerializeField] private StaticContainer panStaticContainer;
    [SerializeField] private PouringSystem pouringSystem;
    [SerializeField] private StirringSystem stirringSystem;

    [SerializeField] private GameObject partOneObjects;
    [SerializeField] private DynamicContainer panPlacementZone;
    [SerializeField] private DynamicContainer onionPlate;
    [SerializeField] private DynamicContainer bellPepperPlate;

    [SerializeField] private GameObject partTwoObjects;

    [SerializeField] private GameObject partThreeObjects;

    [SerializeField] private Instruction cookingSectionInstruction;
    [SerializeField] private Instruction turnStoveToMediumHighInstruction;
    [SerializeField] private Instruction slidePanInstruction;

    [SerializeField] private Instruction partOneInstruction;
    [SerializeField] private Instruction pourOliveOilInstruction;
    [SerializeField] private Instruction oliveOilPouringFailedInstruction;
    [SerializeField] private Instruction addOnionInstruction;
    [SerializeField] private Instruction stirOnionInstruction;
    [SerializeField] private Instruction onionStirringFailedInstruction;
    [SerializeField] private Instruction addBellPepperInstruction;
    [SerializeField] private Instruction stirBellPepperInstruction;
    [SerializeField] private Instruction bellPepperStirringFailedInstruction;
    [SerializeField] private Instruction pourTomatoJuiceInstruction;
    [SerializeField] private Instruction tomatoJuicePouringFailedInstruction;

    [SerializeField] private Instruction partTwoInstruction;
    [SerializeField] private Instruction shakeSaltInstruction;
    [SerializeField] private Instruction shakeGarlicPowderInstruction;
    [SerializeField] private Instruction sprinklePepperInstruction;
    [SerializeField] private Instruction sprinklePaprikaInstruction;
    [SerializeField] private Instruction stirSeasoningInstruction;
    [SerializeField] private Instruction addChickenInstruction;
    [SerializeField] private Instruction stirChickenInstruction;

    [SerializeField] private Instruction partThreeInstruction;
    [SerializeField] private Instruction measureRiceInstruction;
    [SerializeField] private Instruction addRiceInstruction;
    [SerializeField] private Instruction firstMeasureWaterInstruction;
    [SerializeField] private Instruction firstAddWaterInstruction;
    [SerializeField] private Instruction secondMeasureWaterInstruction;
    [SerializeField] private Instruction secondAddWaterInstruction;
    [SerializeField] private Instruction boilLiquidInstruction;
    [SerializeField] private Instruction addShrimpInstruction;
    [SerializeField] private Instruction addMusselsInstruction;
    [SerializeField] private Instruction simmerDownInstruction;
    [SerializeField] private Instruction addLemonInstruction;
    [SerializeField] private Instruction addParsleyInstruction;
    [SerializeField] private Instruction finishedInstruction;

    public override void StartSection()
    {
        base.StartSection();

        cookbook.SetInstruction(cookingSectionInstruction);
        cookbook.ChangeInstructionConfirmationText("Comenzar");
        cookbook.Open();
    }

    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (instruction == cookingSectionInstruction)
        {
            cookbook.SetInstruction(turnStoveToMediumHighInstruction);
            cookbook.ChangeInstructionConfirmationText("Continuar");
        }
        else if (instruction == turnStoveToMediumHighInstruction)
        {
            stove.UnlockKnob();
            stove.OnSettingChanged.Add(OnStoveTurnedOnFirstTime);

            cookbook.Close();
        }
        else if (instruction == slidePanInstruction)
        {
            panDynamicObject.GetComponent<InteractionLocker>().UnlockInteraction();

            panPlacementZone.EnableReceivingObjects();
            panPlacementZone.SetTargetObject(panDynamicObject);
            panPlacementZone.OnObjectAdded.Add(OnPanPlacedOnStove);

            cookbook.Close();
        }
        else if (instruction == partOneInstruction)
        {
            cookbook.SetInstruction(pourOliveOilInstruction);
        }
        else if (instruction == pourOliveOilInstruction || instruction == oliveOilPouringFailedInstruction)
        {
            pouringSystem.OnPouringComplete.Add(OnOliveOilPouringCompleted);
            pouringSystem.OnPouringFailed.Add(OnOliveOilPouringFailed);
            pouringSystem.StartPouring(100, 25);

            cookbook.Close();
        }
        else if (instruction == addOnionInstruction)
        {
            panDynamicContainer.EnableReceivingObjects();
            panDynamicContainer.SetTargetObjects(onionPlate.Objects);
            panDynamicContainer.OnObjectAdded.Add(OnOnionAdded);

            cookbook.Close();
        }
        else if (instruction == stirOnionInstruction || instruction == onionStirringFailedInstruction)
        {
            stirringSystem.OnStirringCompleted.Add(OnOnionStirringCompleted);
            stirringSystem.OnStirringFailed.Add(OnOnionStirringFailed);
            stirringSystem.StartStirring();

            cookbook.Close();
        }
        else if (instruction == addBellPepperInstruction)
        {
            panDynamicContainer.EnableReceivingObjects();
            panDynamicContainer.SetTargetObjects(bellPepperPlate.Objects);
            panDynamicContainer.OnObjectAdded.Add(OnBellPepperAdded);

            cookbook.Close();
        }
        else if (instruction == stirBellPepperInstruction || instruction == bellPepperStirringFailedInstruction)
        {
            stirringSystem.OnStirringCompleted.Add(OnBellPepperStirringCompleted);
            stirringSystem.OnStirringFailed.Add(OnBellPepperStirringFailed);
            stirringSystem.StartStirring();

            cookbook.Close();
        }
        else if (instruction == pourTomatoJuiceInstruction || instruction == tomatoJuicePouringFailedInstruction)
        {
            pouringSystem.OnPouringComplete.Add(OnTomatoJuicePouringCompleted);
            pouringSystem.OnPouringFailed.Add(OnTomatoJuicePouringFailed);
            pouringSystem.StartPouring(1000, 100);

            cookbook.Close();
        }
        else if (instruction == partTwoInstruction)
        {
            partOneObjects.SetActive(false);
            partTwoObjects.SetActive(true);

            cookbook.Close();
        }
    }

    private void OnStoveTurnedOnFirstTime(StoveSetting setting)
    {
        if (setting == StoveSetting.MediumHigh)
        {
            stove.LockKnob();
            stove.OnSettingChanged.Clear();

            cookbook.SetInstruction(slidePanInstruction);
            cookbook.Open();
        }
    }

    private void OnPanPlacedOnStove(DynamicObject _)
    {
        panPlacementZone.OnObjectAdded.Clear();
        panPlacementZone.DisableReceivingObjects();

        cookbook.SetInstruction(partOneInstruction);
        cookbook.Open();
    }

    private void OnOliveOilPouringCompleted()
    {
        pouringSystem.OnPouringComplete.Clear();
        pouringSystem.OnPouringFailed.Clear();

        cookbook.SetInstruction(addOnionInstruction);
        cookbook.Open();
    }

    private void OnOliveOilPouringFailed()
    {
        pouringSystem.OnPouringComplete.Clear();
        pouringSystem.OnPouringFailed.Clear();

        cookbook.SetInstruction(oliveOilPouringFailedInstruction);
        cookbook.Open();
    }

    private void OnOnionAdded(DynamicObject onionObject)
    {
        stirringSystem.TrackObject(onionObject.GetComponent<Stirrable>());

        if (onionPlate.Objects.Count == 0)
        {
            panDynamicContainer.OnObjectAdded.Clear();
            panDynamicContainer.DisableReceivingObjects();

            cookbook.SetInstruction(stirOnionInstruction);
            cookbook.Open();
        }
    }

    private void OnOnionStirringCompleted()
    {
        stirringSystem.OnStirringCompleted.Clear();
        stirringSystem.OnStirringFailed.Clear();

        cookbook.SetInstruction(addBellPepperInstruction);
        cookbook.Open();
    }

    private void OnOnionStirringFailed()
    {
        stirringSystem.OnStirringCompleted.Clear();
        stirringSystem.OnStirringFailed.Clear();

        cookbook.SetInstruction(onionStirringFailedInstruction);
        cookbook.Open();
    }

    private void OnBellPepperAdded(DynamicObject bellPepperObject)
    {
        stirringSystem.TrackObject(bellPepperObject.GetComponent<Stirrable>());

        if (bellPepperPlate.Objects.Count == 0)
        {
            panDynamicContainer.OnObjectAdded.Clear();
            panDynamicContainer.DisableReceivingObjects();

            cookbook.SetInstruction(stirBellPepperInstruction);
            cookbook.Open();
        }
    }

    private void OnBellPepperStirringCompleted()
    {
        stirringSystem.OnStirringCompleted.Clear();
        stirringSystem.OnStirringFailed.Clear();

        cookbook.SetInstruction(pourTomatoJuiceInstruction);
        cookbook.Open();
    }

    private void OnBellPepperStirringFailed()
    {
        stirringSystem.OnStirringCompleted.Clear();
        stirringSystem.OnStirringFailed.Clear();

        cookbook.SetInstruction(bellPepperStirringFailedInstruction);
        cookbook.Open();
    }

    private void OnTomatoJuicePouringCompleted()
    {
        pouringSystem.OnPouringComplete.Clear();
        pouringSystem.OnPouringFailed.Clear();

        cookbook.SetInstruction(partTwoInstruction);
        cookbook.Open();
    }

    private void OnTomatoJuicePouringFailed()
    {
        pouringSystem.OnPouringComplete.Clear();
        pouringSystem.OnPouringFailed.Clear();

        cookbook.SetInstruction(tomatoJuicePouringFailedInstruction);
        cookbook.Open();
    }
}
