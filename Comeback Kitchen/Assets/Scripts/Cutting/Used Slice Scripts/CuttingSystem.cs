using System.Collections.Generic;
using UnityEngine;

public class CuttingSystem : MonoBehaviour
{
    HashSet<SliceableObject> _cutObjectList = new HashSet<SliceableObject>();

    int targetPieceCount = 8;

    public SmartAction OnPhase1Finished = new SmartAction();
    public SmartAction OnPhase2Finished = new SmartAction();

    [SerializeField] Spike spikes;

    public void StartPhase1()
    {
        //
    }


    public void StartPhase2()
    {
        //
    }


    public void ReplaceObject(SliceableObject parent, SliceableObject upper, SliceableObject lower)
    {
        _cutObjectList.Remove(parent);
        _cutObjectList.Add(upper);
        _cutObjectList.Add(lower);

        if(_cutObjectList.Count == targetPieceCount)
        {
            OnPhase2Finished.Invoke();
        }
    }
}
