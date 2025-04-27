using System.Collections.Generic;
using UnityEngine;

public class CookingManager : SectionManager
{
    [SerializeField] private Stove stove;
    [SerializeField] private PlacementZone panPlacementZone;
    [SerializeField] private PanLiquid panLiquid;
    [SerializeField] private Container panFoodItemContainer;
    [SerializeField] private Container onionPlate;
    [SerializeField] private Container bellPepperPlate;

    [SerializeField] private Instruction cookingSectionInstruction;
    [SerializeField] private Instruction turnStoveToMediumHighInstruction;
    [SerializeField] private Instruction slidePanInstruction;

    [SerializeField] private Instruction sauteeVegetablesSubsectionInstruction;
    [SerializeField] private Instruction pourOliveOilInstruction;
    [SerializeField] private Instruction addOnionInstruction;
    [SerializeField] private Instruction stirOnionInstruction;
    [SerializeField] private Instruction addBellPepperInstruction;
    [SerializeField] private Instruction stirBellPepperInstruction;
    [SerializeField] private Instruction addTomatoJuiceInstruction;

    [SerializeField] private Instruction seasoningSubsectionInstruction;
    [SerializeField] private Instruction shakeSaltInstruction;
    [SerializeField] private Instruction shakeGarlicPowderInstruction;
    [SerializeField] private Instruction sprinklePepperInstruction;
    [SerializeField] private Instruction sprinklePaprikaInstruction;
    [SerializeField] private Instruction stirSeasoningInstruction;
    [SerializeField] private Instruction addChickenInstruction;
    [SerializeField] private Instruction stirChickenInstruction;

    [SerializeField] private Instruction finalSubsectionInstruction;
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
        cookbook.Open();
    }

    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (instruction == cookingSectionInstruction)
        {
            cookbook.SetInstruction(turnStoveToMediumHighInstruction);
        }
        else if (instruction == turnStoveToMediumHighInstruction)
        {
            stove.UnlockKnob();
            stove.OnSettingChanged.Add(OnStoveTurnedOnFirstTime);
            cookbook.Close();
        }
        else if (instruction == slidePanInstruction)
        {
            panPlacementZone.gameObject.SetActive(true);
            panPlacementZone.OnObjectEnter.Add(OnPanPlacedOnStove);
            cookbook.Close();
        }
        else if (instruction == sauteeVegetablesSubsectionInstruction)
        {
            cookbook.SetInstruction(pourOliveOilInstruction);
        }
        else if (instruction == pourOliveOilInstruction)
        {
            panLiquid.OnLiquidAdded.Add(OnOliveOilAdded);
            cookbook.Close();
        }
        else if (instruction == addOnionInstruction)
        {
            panFoodItemContainer.OnReceiveObject.Add(OnOnionAdded);
            cookbook.Close();
        }
        else if (instruction == stirOnionInstruction)
        {
            // Skip for now
            cookbook.SetInstruction(addBellPepperInstruction);
        }
        else if (instruction == addBellPepperInstruction)
        {
            panFoodItemContainer.OnReceiveObject.Add(OnBellPepperAdded);
            cookbook.Close();
        }
        else if (instruction == stirBellPepperInstruction)
        {
            // Skip for now
            cookbook.SetInstruction(addTomatoJuiceInstruction);
        }
        else if (instruction == addTomatoJuiceInstruction)
        {
            panLiquid.OnLiquidAdded.Add(OnTomatoJuiceAdded);
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

    private void OnPanPlacedOnStove()
    {
        panPlacementZone.OnObjectEnter.Clear();
        panPlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(sauteeVegetablesSubsectionInstruction);
        cookbook.Open();
    }

    private void OnOliveOilAdded(Dictionary<LiquidType, int> contents)
    {
        if (contents[LiquidType.Oil] >= 50)
        {
            panLiquid.OnLiquidAdded.Clear();
            cookbook.SetInstruction(addOnionInstruction);
            cookbook.Open();
        }
    }

    private void OnOnionAdded(ContainerObject onionObject)
    {
        if (onionPlate.Objects.Count == 0)
        {
            cookbook.SetInstruction(stirOnionInstruction);
            cookbook.Open();
        }
    }

    private void OnBellPepperAdded(ContainerObject bellPepperObject)
    {
        if (bellPepperPlate.Objects.Count == 0)
        {
            cookbook.SetInstruction(stirBellPepperInstruction);
            cookbook.Open();
        }
    }

    private void OnTomatoJuiceAdded(Dictionary<LiquidType, int> contents)
    {
        if (contents[LiquidType.TomatoJuice] >= 1000)
        {
            panLiquid.OnLiquidAdded.Clear();
            cookbook.SetInstruction(seasoningSubsectionInstruction);
            cookbook.Open();
        }
    }
}
