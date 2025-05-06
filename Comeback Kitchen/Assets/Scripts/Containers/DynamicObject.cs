using UnityEngine;

public class DynamicObject : ContainerObject<DynamicObject, DynamicContainer>
{
    public SmartAction<DynamicObject> OnSleep = new SmartAction<DynamicObject>();

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
}