using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ChatGPT
public static class MeshVolumeCalculator
{
    public static float Volume(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        return Mathf.Abs(volume);
    }

    private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Dot(Vector3.Cross(p1, p2), p3) / 6f;
    }




    // Modify for purposes of performance metrics later

    public static bool AreSliceSizesValid(List<GameObject> slices, float maxVolumeDifferencePercentage)
    {
        List<float> volumes = slices.Select(slice => Volume(slice.GetComponent<MeshFilter>().mesh)).ToList();
        float totalVolume = volumes.Sum();

        foreach (float v in volumes)
        {
            float percent = (v / totalVolume) * 100f;
            if (Mathf.Abs(percent - (100f / volumes.Count)) > maxVolumeDifferencePercentage)
                return false;
        }
        

        return true;
    }





    // public static bool ValidSizesWithRatio(List<GameObject> slices, float maxVolumeDifferencePercentage, int ratio)
    // {
    //     List<float> volumes = slices.Select(slice => Volume(slice.GetComponent<MeshFilter>().mesh)).ToList();       //ChatGPT
    //     float totalVolume = volumes.Sum();


    //     float weightedVol1Percent;
    //     float weightedVol2Percent;
        
    //     //if the first mesh's volume is larger, apply the ratio to the volume of the second mesh
    //     if(volumes[0] > volumes[1])
    //     {
    //         weightedVol1Percent = volumes[0] * ratio;
    //         weightedVol2Percent = volumes[1];
            
    //         float volumeDifference = Mathf.Abs(weightedVol1Percent - weightedVol2Percent);

    //         if(volumeDifference > maxVolumeDifferencePercentage)
    //         {
    //             return false;
    //         }

    //     }
    //     else    //otherwise apply the ratio to the volume of the first mesh
    //     {
    //         weightedVol1Percent = volumes[0];
    //         weightedVol2Percent = volumes[1] * ratio;
            
    //         float volumeDifference = Mathf.Abs(weightedVol1Percent - weightedVol2Percent);

    //         if(volumeDifference > maxVolumeDifferencePercentage)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;

    // }


    
}

