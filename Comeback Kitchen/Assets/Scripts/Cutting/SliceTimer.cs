using System.Collections;
using UnityEngine;

public class SliceTimer : MonoBehaviour
{
    private bool recentlySliced;

    void Awake()
    {
        recentlySliced = false;
    }

    public IEnumerator startSliceTimer()
    {
        recentlySliced = true;
        // float num = UnityEngine.Random.Range(1.2f, 2.4f);
        // Debug.Log("Coroutine Wait - " + num);
        yield return new WaitForSeconds(1f);
        // yield return null;
        recentlySliced = false;
    }

    public bool getRecentlySliced()
    {
        return recentlySliced;
    }

}
