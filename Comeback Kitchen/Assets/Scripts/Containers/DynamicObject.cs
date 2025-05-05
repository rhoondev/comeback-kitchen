using UnityEngine;

public class DynamicObject : ContainerObject<DynamicObject, DynamicContainer>
{
    public SmartAction<DynamicObject> OnSettled = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> ReEntered = new SmartAction<DynamicObject>();

    private bool _hasSettled = false;

    private void FixedUpdate()
    {
        // Invoke OnSettled every time the Rigidbody enters sleep mode
        if (!_hasSettled && Rigidbody.IsSleeping())
        {
            _hasSettled = true;
            Debug.Log($"{gameObject.name} has settled.");
            OnSettled.Invoke(this);
        }
        else if (_hasSettled && !Rigidbody.IsSleeping())
        {
            _hasSettled = false;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.transform.parent != null && other.transform.parent.TryGetComponent<DynamicContainer>(out var container) && container == Container)
        {
            ReEntered.Invoke(this);
        }
    }
}