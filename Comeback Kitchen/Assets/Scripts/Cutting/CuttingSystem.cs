using System.Collections.Generic;
using UnityEngine;

public class CuttingSystem : MonoBehaviour
{
    [SerializeField] private Knife knife;
    [SerializeField] private DynamicObject knifeObject;
    [SerializeField] private Spike spikes;
    [SerializeField] private SecondCutZone secondCutZone;
    [SerializeField] private DynamicContainer knifeZone;

    public SmartAction OnIngredientPlaced = new SmartAction();
    public SmartAction OnIngredientChunkRemoved = new SmartAction();
    public SmartAction OnPhase1Finished = new SmartAction();
    public SmartAction OnPhase2Finished = new SmartAction();
    public SmartAction OnKnifePlaced = new SmartAction();

    private int cutCount = 0;
    private int target1PieceCount = 4;
    private int target2PieceCount = 8;

    private void OnEnable()
    {
        knife.OnCut.Add(OnCut);
    }

    private void OnDisable()
    {
        knife.OnCut.Remove(OnCut);
    }

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

        OnIngredientPlaced.Invoke();
    }

    public void OnObjectReadyForCut2()
    {
        secondCutZone.OnObjectEnter.Clear();
        spikes.enabled = false;
        OnIngredientChunkRemoved.Invoke();
    }

    public void OnKnifePutDownPhase1(DynamicObject _)
    {
        knifeZone.ClearTargets();
        knifeZone.OnObjectReceived.Clear();
        OnPhase1Finished.Invoke();
    }

    public void OnKnifePutDownPhase2(DynamicObject _)
    {
        knifeZone.ClearTargets();
        knifeZone.OnObjectReceived.Clear();
        OnPhase2Finished.Invoke();
    }

    public void ReenableSpikes()
    {
        spikes.enabled = true;
    }

    private void OnCut()
    {
        cutCount++;

        if (cutCount == target1PieceCount)
        {
            knifeZone.SetTarget(knifeObject);
            knifeZone.OnObjectReceived.Add(OnKnifePutDownPhase1);
        }
        else if (cutCount == target2PieceCount)
        {
            knifeZone.SetTarget(knifeObject);
            knifeZone.OnObjectReceived.Add(OnKnifePutDownPhase2);
        }
    }
}
