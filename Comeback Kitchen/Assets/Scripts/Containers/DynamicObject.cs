using UnityEngine;

public class DynamicObject : ContainerObject<DynamicObject, DynamicContainer>
{
    public SmartAction<DynamicObject> OnSleep = new SmartAction<DynamicObject>();
    public SmartAction<DynamicObject> OnReEnter = new SmartAction<DynamicObject>();

    private bool _isSleeping = false;

    private void FixedUpdate()
    {
        // Invoke OnSleep every time the Rigidbody enters sleep mode
        if (!_isSleeping && Rigidbody.IsSleeping())
        {
            _isSleeping = true;
            OnSleep.Invoke(this);
        }
        else if (_isSleeping && !Rigidbody.IsSleeping())
        {
            _isSleeping = false;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.transform.parent != null && other.transform.parent.TryGetComponent<DynamicContainer>(out var container) && container == Container)
        {
            OnReEnter.Invoke(this);
        }
    }
}