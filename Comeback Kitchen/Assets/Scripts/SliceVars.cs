using UnityEngine;

public class SliceVars : MonoBehaviour
{

    /*private*/ public bool originalObject;

    /*private*/ public Vector3 originalSize;




    void Awake()
    {
        originalObject = true;
        originalSize = GetComponent<Renderer>().bounds.size;
        Debug.Log("Original Object Size - " + originalSize);
    }



    public Vector3 GetOriginalSize()
    {
        return originalSize;
    }

    public void setOriginalSize(Vector3 originalSize)
    {
        this.originalSize = originalSize;
    }

    public float GetCurrentSize()
    {
        Vector3 objectDimensions = GetComponent<Renderer>().bounds.size;

        float totalVolume = objectDimensions.x * objectDimensions.y * objectDimensions.z;


        return totalVolume;
    }


    public void firstSlice()
    {
        originalObject = false;
        originalSize = GetComponent<Renderer>().bounds.size;
    }


    public bool isFirstSlice()
    {
        return originalObject;
    }

    public void alreadyCut()
    {
        originalObject = false;
    }
}
