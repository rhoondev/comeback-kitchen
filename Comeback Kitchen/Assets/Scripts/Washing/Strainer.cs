using System;
using System.Collections.Generic;
using UnityEngine;

public class Strainer : MonoBehaviour
{
    public SmartAction OnWashedObjectAdded = new SmartAction();

    public int NumTomatoes { get; private set; } = 0;
    public int NumBellPeppers { get; private set; } = 0;

    private List<Washable> _contents = new List<Washable>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Washable washable))
        {
            HandleWashable(washable);
        }
    }

    private void HandleWashable(Washable washable)
    {
        if (washable.IsWashed)
        {
            if (!_contents.Contains(washable))
            {
                // [Release the object from the palyer's hand]
                Rigidbody rb = washable.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                _contents.Add(washable);

                if (washable.gameObject.name == "Tomato")
                {
                    NumTomatoes++;
                }
                else if (washable.gameObject.name == "BellPepper")
                {
                    NumBellPeppers++;
                }

                OnWashedObjectAdded.Invoke();

                Debug.Log("Nice job! You put a washed object in the strainer!");
            }
        }
        else
        {
            Destroy(washable.gameObject);

            // Show error message
            Debug.Log("You must wash the object before putting it in the strainer.");
        }
    }
}
