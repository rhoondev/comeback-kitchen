using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class PreparationManager : SectionManager
{

    private int ingredientTracker = 0;


    [SerializeField] private PlacementZone knifePlacementZone;
    [SerializeField] private PlacementZone onionPlateCollectionSpotPlacementZone;
    [SerializeField] private PlacementZone finalOnionPlatePlacementZone;
    



    // -- Onion Instructions --
    [SerializeField] private Instruction preparationSectionIntroduction;
    [SerializeField] private Instruction onionPlaceOnSpikesInstruction;
    [SerializeField] private Instruction onionInitialCutsInstruction;
    [SerializeField] private Instruction onionPlaceKnife1Instruction;
    [SerializeField] private Instruction onionRemoveFromSpikesInstruction;
    [SerializeField] private Instruction onionFinalCutsInstruction;
    [SerializeField] private Instruction onionPlaceKnife2Instruction;
    [SerializeField] private Instruction onionPutPlateInCollectionPositionInstruction;
    [SerializeField] private Instruction onionPutSlicesOnPlateInstruction;
    [SerializeField] private Instruction onionPlaceKnife3Instruction;
    [SerializeField] private Instruction onionPutFullPlateAwayInstruction;



    
    [SerializeField] private Instruction pepperPlaceOnSpikesInstruction;
    // [SerializeField] private Instruction Instruction;
    // [SerializeField] private Instruction Instruction;
    // [SerializeField] private Instruction Instruction;

    

    
    [SerializeField] private KnifeSlicer knifeInfo;
    [SerializeField] private CuttingSystem cutSystem;

    


    // TODO --- get agle between transform.up and cut angle. Make sure knife is slicing 
    // perpendicular to long way of onion (also need placement zone that is not on spikes for putting onion pieces).
    // Also need to Make objects fall apart when they get into the new placement zone





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
    * - Cut onion into quarters
    * - take onion off of spikes
    * - cut onion pieces again (half every onion piece)
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


    //MUST TURN BLENDER ON BEFORE YOU LET THEM SLICE STUFF




    public override void StartSection()
    {
        // When testing, set the SetInstruction to whatever step I want to test
        base.StartSection();
        cookbook.SetInstruction(preparationSectionIntroduction);
        // cookbook.SetInstruction(onionPutPlateInCollectionPositionInstruction);
        cookbook.Open();
    }



//Logic that happens as soon as a new instruction happens
    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if(ingredientTracker == 0)
        {
            if(instruction == preparationSectionIntroduction)
            {
                cookbook.SetInstruction(onionPlaceOnSpikesInstruction);
            }
            else if (instruction == onionPlaceOnSpikesInstruction)
            {
                // vegetableBasket.SetTargetVegetable("Onion");
                // vegetableBasket.OnVegetableGrabbed.Add(OnOnionGrabbed);
                //Put onion on spikes
                cookbook.Close();
                cutSystem.OnIngredientPlaced.Add(OnOnionPutOnSpikes);
                cutSystem.StartPhase1();
            }
            else if (instruction == onionInitialCutsInstruction)
            {
                // TODO -- set variable/function checking to see if the knife has been picked up (w/ invoke function)
                // something.Add(OnKnifeGrabbed);
                cookbook.Close();
                knifeInfo.firstPhaseCut = true;
                cutSystem.OnPhase1Finished.Add(OnOnionPhase1Completed);

            }
            else if (instruction == onionPlaceKnife1Instruction)
            {
                //tell user to put down knife
                cookbook.Close();
                knifePlacementZone.gameObject.SetActive(true);
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifePlacementZone.OnObjectEnter.Add(OnKnifePlaced1ForOnion);
            }
            else if (instruction == onionRemoveFromSpikesInstruction)
            {
                cookbook.Close();
                cutSystem.OnIngredientChunkRemoved.Add(OnOnionReadyForCut2);
                cutSystem.StartPhase2();
            }
            else if(instruction == onionFinalCutsInstruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                knifeInfo.firstPhaseCut = false;
                cutSystem.OnPhase2Finished.Add(OnOnionPhase2Completed);
            }
            else if(instruction == onionPlaceKnife2Instruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifePlacementZone.gameObject.SetActive(true);
                knifePlacementZone.OnObjectEnter.Add(OnKnifePlaced2ForOnion);
            }
            else if(instruction == onionPutPlateInCollectionPositionInstruction)
            {
                cookbook.Close();
                onionPlateCollectionSpotPlacementZone.gameObject.SetActive(true);
                onionPlateCollectionSpotPlacementZone.OnObjectEnter.Add(OnPlateInCollectionPosition);
            }
            else if(instruction == onionPutSlicesOnPlateInstruction)
            {
                cookbook.Close();
                //something.Add(OnOnionSlicesInPlate)
                // TODO -- this is where container stuff should be added
            }
            else if(instruction == onionPlaceKnife3Instruction)
            {
                cookbook.Close();
                knifePlacementZone.OnObjectEnter.Add(OnKnifePlaced3ForOnion);
                knifePlacementZone.gameObject.SetActive(true);
            }
            else if(instruction == onionPutFullPlateAwayInstruction)
            {
                cookbook.Close();
                finalOnionPlatePlacementZone.OnObjectEnter.Add(OnOnionTasksDone);
                finalOnionPlatePlacementZone.gameObject.SetActive(true);
            }
        }
        else if(ingredientTracker == 1)
        {
        // else if(instruction == )
        // {
        //     //
        // }
        // else if(instruction == )
        // {
        //     //
        // }
        }
        else if(ingredientTracker == 2)
        {
            //
        }
    }




























    //    ------------------ Onion Preparation ------------------

    public void OnOnionPutOnSpikes()
    {
        //Clear SmartAction so that the function is not randomly called again
        cutSystem.OnIngredientPlaced.Clear();
        cookbook.SetInstruction(onionInitialCutsInstruction);      //Move on to the next instruction step
        cookbook.Open();                                    //Open the cookbook
    }


    private void OnOnionPhase1Completed()
    {
        cutSystem.OnPhase1Finished.Clear();
        cookbook.SetInstruction(onionPlaceKnife1Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced1ForOnion(GameObject _)
    {
        knifePlacementZone.OnObjectEnter.Clear();
        knifePlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionRemoveFromSpikesInstruction);
        cookbook.Open();
    }

    private void OnOnionReadyForCut2()
    {
        cutSystem.OnIngredientChunkRemoved.Clear();
        cookbook.SetInstruction(onionFinalCutsInstruction);
        cookbook.Open();
    }

    private void OnOnionPhase2Completed()
    {
        cutSystem.OnPhase2Finished.Clear();
        cookbook.SetInstruction(onionPlaceKnife2Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced2ForOnion(GameObject _)
    {
        knifePlacementZone.OnObjectEnter.Clear();
        knifePlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionPutPlateInCollectionPositionInstruction);
        cookbook.Open();
    }

    private void OnPlateInCollectionPosition(GameObject _)
    {
        onionPlateCollectionSpotPlacementZone.OnObjectEnter.Clear();
        onionPlateCollectionSpotPlacementZone.gameObject.SetActive(false);
        knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        cookbook.SetInstruction(onionPutSlicesOnPlateInstruction);
        cookbook.Open();
    }


    private void OnOnionSlicesInPlate()
    {
        //TODO -- container logic perhaps?
        cookbook.SetInstruction(onionPlaceKnife3Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced3ForOnion(GameObject _)
    {
        knifePlacementZone.OnObjectEnter.Clear();
        knifePlacementZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionPutFullPlateAwayInstruction);
        cookbook.Open();
    }

    
    private void OnOnionTasksDone(GameObject _)
    {
        finalOnionPlatePlacementZone.OnObjectEnter.Clear();
        finalOnionPlatePlacementZone.gameObject.SetActive(false);
        ingredientTracker++;
        cutSystem.ReenableSpikes();
        cookbook.SetInstruction(pepperPlaceOnSpikesInstruction);
        cookbook.Open();
    }

    




    
    // //    ------------------ Pepper Preparation ------------------

    // public void OnPepperPutOnSpikes()
    // {
    //     // spikePlacementZone.OnObjectEnter.Clear();           //Clear SmartAction so that the function is not randomly called again
    //     cookbook.SetInstruction(grabKnifeInstruction);      //Move on to the next instruction step
    //     cookbook.Open();                                    //Open the cookbook
    // }









    // //    ------------------ Tomato Preparation ------------------

    // public void OnTomatoPutOnSpikes()
    // {
    //     // spikePlacementZone.OnObjectEnter.Clear();           //Clear SmartAction so that the function is not randomly called again
    //     cookbook.SetInstruction(grabKnifeInstruction);      //Move on to the next instruction step
    //     cookbook.Open();                                    //Open the cookbook
    // }




}
