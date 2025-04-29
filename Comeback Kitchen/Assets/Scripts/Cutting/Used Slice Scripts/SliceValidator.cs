using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//ChatGPT
public class SliceValidator : MonoBehaviour
{
    public float maxVolumeDifferencePercentage = 15f; // e.g., slices must be within 15% of each other

    public bool AreSlicesSimilar(List<GameObject> slices)
    {
        List<float> volumes = slices.Select(slice => MeshVolumeCalculator.Volume(slice.GetComponent<MeshFilter>().mesh)).ToList();
        float totalVolume = volumes.Sum();

        foreach (float v in volumes)
        {
            float percent = (v / totalVolume) * 100f;
            if (Mathf.Abs(percent - (100f / volumes.Count)) > maxVolumeDifferencePercentage)
                return false;
        }

        return true;
    }
}
