using System;
using System.Collections.Generic;
using UnityEngine;

public class Washable : MonoBehaviour
{
    [SerializeField] private SimpleProgressBar progressBar;
    [SerializeField] private int washQuota;

    public SmartAction<Washable> OnWashed = new SmartAction<Washable>();

    private int _amountWashed = 0;
    private bool _isClean = false;

    public void HideProgressBar()
    {
        progressBar.gameObject.SetActive(false);
    }

    private void Wash(int amount)
    {
        if (_amountWashed == 0)
        {
            progressBar.gameObject.SetActive(true); // Enable progress bar upon starting to wash the item
        }

        _amountWashed += amount;
        progressBar.SetValue((float)Math.Min(_amountWashed, washQuota) / washQuota);

        if (!_isClean && _amountWashed >= washQuota)
        {
            _isClean = true;

            Debug.Log($"{gameObject.name} is now clean!");

            OnWashed.Invoke(this);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<Stream>(out var _))
        {
            var collisionEvents = new List<ParticleCollisionEvent>();
            other.GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
            Wash(collisionEvents.Count);
        }
    }
}
