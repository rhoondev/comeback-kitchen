using UnityEngine;

public class PreparationManager : SectionManager
{
    // -- Placement Zones --
    [Header("Misc Info")]
    [Space(10)]

    private int ingredientTracker = 0;
    [SerializeField] private KnifeSlicer knifeInfo;
    [SerializeField] private CuttingSystem cuttingSystem;

    // -- Placement Zones --
    [Header("Zones")]
    [Space(10)]

    [SerializeField] private DynamicContainer knifeZone; // This is where the knife is picked up and put down

    [SerializeField] private DynamicContainer initialOnionPlateZone; // This is where the onion plate is in the stack of plates
    [SerializeField] private DynamicContainer initialTomatoPlateZone; // This is where the tomato plate is in the stack of plates
    [SerializeField] private DynamicContainer initialBellPepperPlateZone; // This is where the bell pepper plate is in the stack of plates

    [SerializeField] private DynamicContainer activePlateZone; // This is where each plate is put after cutting the associated vegetable and before sliding the pieces onto the plate

    [SerializeField] private DynamicContainer finalOnionPlateZone; // This is where the onion plate is put after cutting
    [SerializeField] private DynamicContainer finalTomatoPlateZone; // This is where the tomato plate is put after cutting
    [SerializeField] private DynamicContainer finalBellPepperPlateZone;
    // The bell pepper plate is left in the collection zone, so no need for a final bell pepper plate zone




    // -- Onion Instructions --
    [Header("Onion Instructions")]
    [Space(10)]

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




    // -- Pepper Instructions --
    [Header("Pepper Instructions")]
    [Space(10)]

    [SerializeField] private Instruction pepperPlaceOnSpikesInstruction;
    [SerializeField] private Instruction pepperInitialCutsInstruction;
    [SerializeField] private Instruction pepperPlaceKnife1Instruction;
    [SerializeField] private Instruction pepperRemoveFromSpikesInstruction;
    [SerializeField] private Instruction pepperFinalCutsInstruction;
    [SerializeField] private Instruction pepperPlaceKnife2Instruction;
    [SerializeField] private Instruction pepperPutPlateInCollectionPositionInstruction;
    [SerializeField] private Instruction pepperPutSlicesOnPlateInstruction;
    [SerializeField] private Instruction pepperPlaceKnife3Instruction;
    [SerializeField] private Instruction pepperPutFullPlateAwayInstruction;




    // -- Tomato Instructions --
    [Header("Tomato Instructions")]
    [Space(10)]

    [SerializeField] private Instruction tomatoPlaceOnSpikesInstruction;
    [SerializeField] private Instruction tomatoInitialCutsInstruction;
    [SerializeField] private Instruction tomatoPlaceKnife1Instruction;
    [SerializeField] private Instruction tomatoRemoveFromSpikesInstruction;
    [SerializeField] private Instruction tomatoFinalCutsInstruction;
    [SerializeField] private Instruction tomatoPlaceKnife2Instruction;
    [SerializeField] private Instruction tomatoPutPlateInCollectionPositionInstruction;
    [SerializeField] private Instruction tomatoPutSlicesOnPlateInstruction;
    [SerializeField] private Instruction tomatoPlaceKnife3Instruction;
    [SerializeField] private Instruction tomatoPutFullPlateAwayInstruction;



    // -- Tomato Blending Instructions --
    [Header("Tomato Blending Instructions")]
    [Space(10)]

    [SerializeField] private Instruction tomatoStartBlendInstruction;








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
        base.StartSection();
        cookbook.SetInstruction(preparationSectionIntroduction);
        cookbook.ChangeInstructionConfirmationText("Comenzar");
        cookbook.Open();
    }



    //Logic that happens as soon as a new instruction happens
    protected override void OnConfirmInstruction(Instruction instruction)
    {
        if (ingredientTracker == 0)
        {
            if (instruction == preparationSectionIntroduction)
            {
                cookbook.SetInstruction(onionPlaceOnSpikesInstruction);
                cookbook.ChangeInstructionConfirmationText("Continuar");
            }
            else if (instruction == onionPlaceOnSpikesInstruction)
            {
                // vegetableBasket.SetTargetVegetable("Onion");
                // vegetableBasket.OnVegetableGrabbed.Add(OnOnionGrabbed);
                //Put onion on spikes
                cookbook.Close();
                cuttingSystem.OnIngredientPlaced.Add(OnOnionPutOnSpikes);
                cuttingSystem.StartPhase1();
            }
            else if (instruction == onionInitialCutsInstruction)
            {
                // TODO -- set variable/function checking to see if the knife has been picked up (w/ invoke function)
                // something.Add(OnKnifeGrabbed);
                cookbook.Close();
                knifeInfo.FirstPhaseCut = true;
                cuttingSystem.OnPhase1Finished.Add(OnOnionPhase1Completed);

            }
            else if (instruction == onionPlaceKnife1Instruction)
            {
                //tell user to put down knife
                cookbook.Close();
                knifeZone.gameObject.SetActive(true);
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.OnObjectReceived.Add(OnKnifePlaced1ForOnion);
            }
            else if (instruction == onionRemoveFromSpikesInstruction)
            {
                cookbook.Close();
                cuttingSystem.OnIngredientChunkRemoved.Add(OnOnionReadyForCut2);
                cuttingSystem.StartPhase2();
            }
            else if (instruction == onionFinalCutsInstruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                knifeInfo.FirstPhaseCut = false;
                cuttingSystem.OnPhase2Finished.Add(OnOnionPhase2Completed);
            }
            else if (instruction == onionPlaceKnife2Instruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.gameObject.SetActive(true);
                knifeZone.OnObjectReceived.Add(OnKnifePlaced2ForOnion);
            }
            else if (instruction == onionPutPlateInCollectionPositionInstruction)
            {
                cookbook.Close();
                activePlateZone.gameObject.SetActive(true);
                activePlateZone.OnObjectReceived.Add(OnOnionPlateInCollectionPosition);
            }
            else if (instruction == onionPutSlicesOnPlateInstruction)
            {
                cookbook.Close();
                //something.Add(OnOnionSlicesInPlate)
                // TODO -- this is where container stuff should be added
            }
            else if (instruction == onionPlaceKnife3Instruction)
            {
                cookbook.Close();
                knifeZone.OnObjectReceived.Add(OnKnifePlaced3ForOnion);
                knifeZone.gameObject.SetActive(true);
            }
            else if (instruction == onionPutFullPlateAwayInstruction)
            {
                cookbook.Close();
                finalOnionPlateZone.OnObjectReceived.Add(OnOnionTasksDone);
                finalOnionPlateZone.gameObject.SetActive(true);
                cuttingSystem.ReenableSpikes();                                             //TODO -- check if spikes actually are reenabled by here
            }
        }
        else if (ingredientTracker == 1)
        {
            if (instruction == tomatoPlaceOnSpikesInstruction)
            {
                // vegetableBasket.SetTargetVegetable("tomato");
                // vegetableBasket.OnVegetableGrabbed.Add(OntomatoGrabbed);
                //Put tomato on spikes
                cookbook.Close();
                cuttingSystem.OnIngredientPlaced.Add(OnTomatoPutOnSpikes);
                cuttingSystem.StartPhase1();
            }
            else if (instruction == tomatoInitialCutsInstruction)
            {
                // TODO -- set variable/function checking to see if the knife has been picked up (w/ invoke function)
                // something.Add(OnKnifeGrabbed);
                cookbook.Close();
                knifeInfo.FirstPhaseCut = true;
                cuttingSystem.OnPhase1Finished.Add(OnTomatoPhase1Completed);

            }
            else if (instruction == tomatoPlaceKnife1Instruction)
            {
                //tell user to put down knife
                cookbook.Close();
                knifeZone.gameObject.SetActive(true);
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.OnObjectReceived.Add(OnKnifePlaced1ForTomato);
            }
            else if (instruction == tomatoRemoveFromSpikesInstruction)
            {
                cookbook.Close();
                cuttingSystem.OnIngredientChunkRemoved.Add(OnTomatoReadyForCut2);
                cuttingSystem.StartPhase2();
            }
            else if (instruction == tomatoFinalCutsInstruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                knifeInfo.FirstPhaseCut = false;
                cuttingSystem.OnPhase2Finished.Add(OnTomatoPhase2Completed);
            }
            else if (instruction == tomatoPlaceKnife2Instruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.gameObject.SetActive(true);
                knifeZone.OnObjectReceived.Add(OnKnifePlaced2ForTomato);
            }
            else if (instruction == tomatoPutPlateInCollectionPositionInstruction)
            {
                cookbook.Close();
                activePlateZone.gameObject.SetActive(true);
                activePlateZone.OnObjectReceived.Add(OnTomatoPlateInCollectionPosition);
            }
            else if (instruction == tomatoPutSlicesOnPlateInstruction)
            {
                cookbook.Close();
                //something.Add(OntomatoSlicesInPlate)
                // TODO -- this is where container stuff should be added
            }
            else if (instruction == tomatoPlaceKnife3Instruction)
            {
                cookbook.Close();
                knifeZone.OnObjectReceived.Add(OnKnifePlaced3ForTomato);
                knifeZone.gameObject.SetActive(true);
            }
            else if (instruction == tomatoPutFullPlateAwayInstruction)
            {
                cookbook.Close();
                finalTomatoPlateZone.OnObjectReceived.Add(OnTomatoTasksDone);
                finalTomatoPlateZone.gameObject.SetActive(true);
                cuttingSystem.ReenableSpikes();                                             //TODO -- check if spikes actually are reenabled by here
            }
        }
        else if (ingredientTracker == 2)
        {
            if (instruction == pepperPlaceOnSpikesInstruction)
            {
                // vegetableBasket.SetTargetVegetable("pepper");
                // vegetableBasket.OnVegetableGrabbed.Add(OnpepperGrabbed);
                //Put pepper on spikes
                cookbook.Close();
                cuttingSystem.OnIngredientPlaced.Add(OnPepperPutOnSpikes);
                cuttingSystem.StartPhase1();
            }
            else if (instruction == pepperInitialCutsInstruction)
            {
                // TODO -- set variable/function checking to see if the knife has been picked up (w/ invoke function)
                // something.Add(OnKnifeGrabbed);
                cookbook.Close();
                knifeInfo.FirstPhaseCut = true;
                cuttingSystem.OnPhase1Finished.Add(OnPepperPhase1Completed);

            }
            else if (instruction == pepperPlaceKnife1Instruction)
            {
                //tell user to put down knife
                cookbook.Close();
                knifeZone.gameObject.SetActive(true);
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.OnObjectReceived.Add(OnKnifePlaced1ForPepper);
            }
            else if (instruction == pepperRemoveFromSpikesInstruction)
            {
                cookbook.Close();
                cuttingSystem.OnIngredientChunkRemoved.Add(OnPepperReadyForCut2);
                cuttingSystem.StartPhase2();
            }
            else if (instruction == pepperFinalCutsInstruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                knifeInfo.FirstPhaseCut = false;
                cuttingSystem.OnPhase2Finished.Add(OnPepperPhase2Completed);
            }
            else if (instruction == pepperPlaceKnife2Instruction)
            {
                cookbook.Close();
                knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                knifeZone.gameObject.SetActive(true);
                knifeZone.OnObjectReceived.Add(OnKnifePlaced2ForPepper);
            }
            else if (instruction == pepperPutPlateInCollectionPositionInstruction)
            {
                cookbook.Close();
                activePlateZone.gameObject.SetActive(true);
                activePlateZone.OnObjectReceived.Add(OnPepperPlateInCollectionPosition);
            }
            else if (instruction == pepperPutSlicesOnPlateInstruction)
            {
                cookbook.Close();
                //something.Add(OnpepperSlicesInPlate)
                // TODO -- this is where container stuff should be added
            }
            else if (instruction == pepperPlaceKnife3Instruction)
            {
                cookbook.Close();
                knifeZone.OnObjectReceived.Add(OnKnifePlaced3ForPepper);
                knifeZone.gameObject.SetActive(true);
            }
            else if (instruction == pepperPutFullPlateAwayInstruction)
            {
                cookbook.Close();
                finalBellPepperPlateZone.OnObjectReceived.Add(OnPepperTasksDone);
                finalBellPepperPlateZone.gameObject.SetActive(true);
                cuttingSystem.ReenableSpikes();                                             // TODO -- check if spikes actually are reenabled by here
            }
        }
        else if (ingredientTracker == 3)
        {
            //Tomato Blending Stuff
        }
    }




























    //    ------------------ Onion Preparation ------------------

    public void OnOnionPutOnSpikes()
    {
        //Clear SmartAction so that the function is not randomly called again
        cuttingSystem.OnIngredientPlaced.Clear();
        cookbook.SetInstruction(onionInitialCutsInstruction);      //Move on to the next instruction step
        cookbook.Open();                                    //Open the cookbook
    }


    private void OnOnionPhase1Completed()
    {
        cuttingSystem.OnPhase1Finished.Clear();
        cookbook.SetInstruction(onionPlaceKnife1Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced1ForOnion(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionRemoveFromSpikesInstruction);
        cookbook.Open();
    }

    private void OnOnionReadyForCut2()
    {
        cuttingSystem.OnIngredientChunkRemoved.Clear();
        cookbook.SetInstruction(onionFinalCutsInstruction);
        cookbook.Open();
    }

    private void OnOnionPhase2Completed()
    {
        cuttingSystem.OnPhase2Finished.Clear();
        cookbook.SetInstruction(onionPlaceKnife2Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced2ForOnion(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionPutPlateInCollectionPositionInstruction);
        cookbook.Open();
    }

    private void OnOnionPlateInCollectionPosition(DynamicObject _)
    {
        activePlateZone.OnObjectReceived.Clear();
        activePlateZone.gameObject.SetActive(false);
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

    private void OnKnifePlaced3ForOnion(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(onionPutFullPlateAwayInstruction);
        cookbook.Open();
    }


    private void OnOnionTasksDone(DynamicObject _)
    {
        finalOnionPlateZone.OnObjectReceived.Clear();
        finalOnionPlateZone.gameObject.SetActive(false);
        ingredientTracker++;
        cuttingSystem.ReenableSpikes();
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
    public void OnPepperPutOnSpikes()
    {
        //Clear SmartAction so that the function is not randomly called again
        cuttingSystem.OnIngredientPlaced.Clear();
        cookbook.SetInstruction(pepperInitialCutsInstruction);      //Move on to the next instruction step
        cookbook.Open();                                    //Open the cookbook
    }


    private void OnPepperPhase1Completed()
    {
        cuttingSystem.OnPhase1Finished.Clear();
        cookbook.SetInstruction(pepperPlaceKnife1Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced1ForPepper(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(pepperRemoveFromSpikesInstruction);
        cookbook.Open();
    }

    private void OnPepperReadyForCut2()
    {
        cuttingSystem.OnIngredientChunkRemoved.Clear();
        cookbook.SetInstruction(pepperFinalCutsInstruction);
        cookbook.Open();
    }

    private void OnPepperPhase2Completed()
    {
        cuttingSystem.OnPhase2Finished.Clear();
        cookbook.SetInstruction(pepperPlaceKnife2Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced2ForPepper(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(pepperPutPlateInCollectionPositionInstruction);
        cookbook.Open();
    }

    private void OnPepperPlateInCollectionPosition(DynamicObject _)
    {
        activePlateZone.OnObjectReceived.Clear();
        activePlateZone.gameObject.SetActive(false);
        knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        cookbook.SetInstruction(pepperPutSlicesOnPlateInstruction);
        cookbook.Open();
    }


    private void OnPepperSlicesInPlate()
    {
        //TODO -- container logic perhaps?
        cookbook.SetInstruction(pepperPlaceKnife3Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced3ForPepper(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(pepperPutFullPlateAwayInstruction);
        cookbook.Open();
    }


    private void OnPepperTasksDone(DynamicObject _)
    {
        finalBellPepperPlateZone.OnObjectReceived.Clear();
        finalBellPepperPlateZone.gameObject.SetActive(false);
        ingredientTracker++;
        cuttingSystem.ReenableSpikes();
        cookbook.SetInstruction(tomatoPlaceOnSpikesInstruction);
        cookbook.Open();
    }










    // //    ------------------ Tomato Preparation ------------------

    // public void OnTomatoPutOnSpikes()
    // {
    //     // spikePlacementZone.OnObjectEnter.Clear();           //Clear SmartAction so that the function is not randomly called again
    //     cookbook.SetInstruction(grabKnifeInstruction);      //Move on to the next instruction step
    //     cookbook.Open();                                    //Open the cookbook
    // }
    public void OnTomatoPutOnSpikes()
    {
        //Clear SmartAction so that the function is not randomly called again
        cuttingSystem.OnIngredientPlaced.Clear();
        cookbook.SetInstruction(tomatoInitialCutsInstruction);      //Move on to the next instruction step
        cookbook.Open();                                    //Open the cookbook
    }


    private void OnTomatoPhase1Completed()
    {
        cuttingSystem.OnPhase1Finished.Clear();
        cookbook.SetInstruction(tomatoPlaceKnife1Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced1ForTomato(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(tomatoRemoveFromSpikesInstruction);
        cookbook.Open();
    }

    private void OnTomatoReadyForCut2()
    {
        cuttingSystem.OnIngredientChunkRemoved.Clear();
        cookbook.SetInstruction(tomatoFinalCutsInstruction);
        cookbook.Open();
    }

    private void OnTomatoPhase2Completed()
    {
        cuttingSystem.OnPhase2Finished.Clear();
        cookbook.SetInstruction(tomatoPlaceKnife2Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced2ForTomato(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(tomatoPutPlateInCollectionPositionInstruction);
        cookbook.Open();
    }

    private void OnTomatoPlateInCollectionPosition(DynamicObject _)
    {
        activePlateZone.OnObjectReceived.Clear();
        activePlateZone.gameObject.SetActive(false);
        knifeInfo.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        knifeInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        cookbook.SetInstruction(tomatoPutSlicesOnPlateInstruction);
        cookbook.Open();
    }


    private void OnTomatoSlicesInPlate()
    {
        //TODO -- container logic perhaps?
        cookbook.SetInstruction(tomatoPlaceKnife3Instruction);
        cookbook.Open();
    }

    private void OnKnifePlaced3ForTomato(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        knifeZone.gameObject.SetActive(false);
        cookbook.SetInstruction(tomatoPutFullPlateAwayInstruction);
        cookbook.Open();
    }


    private void OnTomatoTasksDone(DynamicObject _)
    {
        finalTomatoPlateZone.OnObjectReceived.Clear();
        finalTomatoPlateZone.gameObject.SetActive(false);
        ingredientTracker++;
        cuttingSystem.ReenableSpikes();
        cookbook.SetInstruction(tomatoStartBlendInstruction);
        cookbook.Open();
    }



}
