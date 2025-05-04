using System.Collections.Generic;
using UnityEngine;

public class CuttingSystem : MonoBehaviour
{
    HashSet<SliceableObject> _cutObjectList = new HashSet<SliceableObject>();

    int target1PieceCount = 4;
    int target2PieceCount = 8;

    public SmartAction OnIngredientPlaced = new SmartAction();
    public SmartAction OnIngredientChunkRemoved = new SmartAction();
    public SmartAction OnPhase1Finished = new SmartAction();
    public SmartAction OnPhase2Finished = new SmartAction();
    public SmartAction OnKnifePlaced = new SmartAction();



    [SerializeField] private DynamicObject _knife;


    [SerializeField] Spike spikes;
    [SerializeField] SecondCutZone secondCutZone;



    [SerializeField] private DynamicContainer knifeZone;




    public void StartPhase1()
    {
        spikes.OnObjectEnter.Add(OnObjectPlaceOnSpikes);
    }


    public void StartPhase2()
    {
        secondCutZone.OnObjectEnter.Add(OnObjectReadyForCut2);
    }



    public void OnObjectPlaceOnSpikes()
    {
        spikes.OnObjectEnter.Clear();
        //Enable ability to pick up knife

        //Phase 1 goes from putting object on spikes to when object is in 4 pieces
        if (_cutObjectList.Count == 4)
            OnPhase1Finished.Invoke();
        else if (_cutObjectList.Count == 0)
        {
            OnIngredientPlaced.Invoke();
        }

    }

    public void OnObjectReadyForCut2()
    {
        secondCutZone.OnObjectEnter.Clear();
        spikes.enabled = false;
        OnIngredientChunkRemoved.Invoke();
    }



    public void OnKnifePutDown(DynamicObject _)
    {
        knifeZone.OnObjectReceived.Clear();
        OnPhase1Finished.Invoke();
    }



    public void ReenableSpikes()
    {
        spikes.enabled = true;
    }






    // Called whenever an object is sliced by KnifeSlicer Script

    public void ReplaceObject(SliceableObject parent, SliceableObject upper, SliceableObject lower)
    {
        Debug.Log("Replace Object called");
        //Adding and removing from a list (different than adding to a SmartAction)
        _cutObjectList.Remove(parent);
        _cutObjectList.Add(upper);
        _cutObjectList.Add(lower);

        if (_cutObjectList.Count == target1PieceCount)
        {
            //Make user put down knife
            knifeZone.gameObject.SetActive(true);
            knifeZone.SetTargetObject(_knife);
            knifeZone.OnObjectReceived.Add(OnKnifePutDown);
            OnPhase1Finished.Invoke();
        }
        else if (_cutObjectList.Count == target2PieceCount)
        {
            OnPhase2Finished.Invoke();
        }

    }
}
