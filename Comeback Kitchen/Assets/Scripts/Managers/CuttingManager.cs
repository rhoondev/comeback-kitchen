using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class CuttingManager : SectionManager
{


    [SerializeField] private PlacementZone knifePlacementZone;
    [SerializeField] private PlacementZone spikePlacementZone;
    
    [SerializeField] private Instruction preparationSectionIntroduction;
    [SerializeField] private Instruction grabAndPlaceOnionInstruction;
    [SerializeField] private Instruction grabKnifeInstruction;
    [SerializeField] private Instruction firstSliceInstruction;
    [SerializeField] private Instruction secondSliceInstruction;
    [SerializeField] private Instruction thirdSliceInstruction;
    [SerializeField] private Instruction placeKnifeInstruction;
    [SerializeField] private Instruction grabPlateInstruction;
    // [SerializeField] private Instruction Instruction;
    // [SerializeField] private Instruction Instruction;
    // [SerializeField] private Instruction Instruction;

    
    [SerializeField] private VegetableBasket vegetableBasket;

    
    private GameObject _onion;
    private GameObject _tomato;
    private GameObject _bellPepper;
    private GameObject _knife;



    /*
    unordered container on the plate
     use plate prefab



    For cutting
    knife looks at bounding and position of knife
    if knife is at correct world spot and also at correct spot on bounding box (world spot is mapped to bounding box), then let cut succeed, otherwise fail
    do this 3 times
    */

    /*
    Cuttng will have multiple functions/ steps within each function
    */

    /*
    *  -----   Steps   -----
    * 1) Welcome user to the section
    *
    * -- Onion --
    * 2) User must grab an onion and place it on the spikes
    * 3) User picks up the knife (lock it in their hands)
    * 4) Cut the onion into desired number of slices (then call volume checker function)
    * 5) Put the knife down
    * 6) Grab a plate and place it next to the spikes
    * 7) Take onion off of spikes (onion falls apart)
    * 8) Pick up the knife
    * 9) Push onion pieces onto the plate (need to learn how container class works so onion stuff can be saved)
    * 10) Put down knife
    * 11) grab plate and place in correct spot (plate locked in hand until valid spot)
    *
    * -- Pepper --
    * 12) User must grab a red pepper and place it on the spikes
    * 13) User picks up the knife (lock it in their hands)
    * 14) Cut the red pepper into desired number of slices (then call volume checker function)
    * 15) Put the knife down
    * 16) Grab a plate and place it next to the spikes
    * 17) Take the red pepper off of the spikes (pepper falls apart)
    * 18) Pick up the knife
    * 19) Push the red pepper pieces onto the plate
    * 20) Put down the knife
    * 21) grab plate and place in correct spot (plate locked in hand until valid spot)
    *
    * -- Tomato --
    * 22) User must grab a tomato and place it on the spikes
    * 23) User picks up the knife (lock it in their hands)
    * 24) Cut the tomato into desired number of slices (then call volume checker function)
    * 25) Put the knife down
    * 26) Grab a plate and place it next to the spikes
    * 27) Take the tomato off of the spikes (tomato falls apart)
    * 28) Pick up the knife
    * 29) Push the tomato onto the plate
    * 30) Put the knife down
    * 31) grab plate and place in correct spot (plate locked in hand until valid spot)
    * 
    * -- Tomato Blending --
    * 32) Remove Blender Lid
    * 33) Take Tomato slices and put into blender
    * 34) Put lid back on blender
    * 35) turn blender on
    * 36) Pick up the pitcher and place in correct spot
    *
    *  -----   Phase Completed   -----
    * 37) Send congratulatory message and go to next stage
    *
    */

    public override void StartSection()
    {
        // When testing, set the SetInstruction to whatever step I want to test
        base.StartSection();
        cookbook.SetInstruction(preparationSectionIntroduction);
        cookbook.Open();
    }



//Logic that happens as soon as a new instruction happens
    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if(instruction == preparationSectionIntroduction)
        {
            cookbook.SetInstruction(grabAndPlaceOnionInstruction);
        }
        else if (instruction == grabAndPlaceOnionInstruction)
        {
            vegetableBasket.SetTargetVegetable("Onion");
            vegetableBasket.OnVegetableGrabbed.Add(OnOnionGrabbed);
            cookbook.Close();
        }
        else if (instruction == grabKnifeInstruction)
        {
            // TODO -- set variable/function checking to see if the knife has been picked up (w/ invoke function)
            something.Add(OnKnifeGrabbed);
            cookbook.Close();
        }
        else if (instruction == firstSliceInstruction)
        {
            //Instruct user to cut the object in half
            something.Add(OnFirstSlice);
            cookbook.Close();
        }
        else if (instruction == secondSliceInstruction)
        {
            //Instruct user to cut the object in half
            something.Add(OnSecondSlice);
            cookbook.Close();
        }
        else if (instruction == thirdSliceInstruction)
        {
            //Instruct user to cut the object in half
            something.Add(OnThirdSlice);
            cookbook.Close();
        }
        else if (instruction == placeKnifeInstruction)
        {
            knifePlacementZone.gameObject.SetActive(true);
            knifePlacementZone.SetTargetObject(_tomato);
            knifePlacementZone.OnObjectEnter.Add(OnKnifePutDown);
            cookbook.Close();
        }
        else if()
        {
            //
        }
    }








    //Functions that are called when condition is true

    private void OnOnionGrabbed(GameObject onion)
    {
        vegetableBasket.OnVegetableGrabbed.Clear();
        _onion = onion;       //set onion variable to grabbed onion object
        spikePlacementZone.gameObject.SetActive(true);       //turn on the placement zone image (highlighted area)
        spikePlacementZone.SetTargetObject(_onion);         //Set the zone target object
        spikePlacementZone.OnObjectEnter.Add(OnOnionPutOnSpikes);       //SmartAction call, when invoked by the onion entering the area, will trigger the function OnOnionPutOnSpikes()
    }

    private void OnOnionPutOnSpikes()
    {
        spikePlacementZone.OnObjectEnter.Clear();           //Clear SmartAction so that the function is not randomly called again
        cookbook.SetInstruction(grabKnifeInstruction);      //Move on to the next instruction step
        cookbook.Open();                                    //Open the cookbook
    }

    private void OnKnifeGrabbed(GameObject knife)
    {
        // TODO -- lock the knife in the user's hand (by making it a child)
        
        cookbook.SetInstruction(firstSliceInstruction);
        cookbook.Open();
    }



    private void OnFirstSlice()
    {
        // check slices here perhaps?
        cookbook.SetInstruction(secondSliceInstruction);
        cookbook.Open();
    }


    private void OnSecondSlice()
    {
        // check slices here perhaps?
        cookbook.SetInstruction(thirdSliceInstruction);
        cookbook.Open();
    }


    private void OnThirdSlice()
    {
        // check slices here perhaps?
        cookbook.SetInstruction(placeKnifeInstruction);
        cookbook.Open();
    }



    private void OnKnifePutDown()
    {
        //
        cookbook.SetInstruction(placeKnifeInstruction);
        cookbook.Open();
    }






}
