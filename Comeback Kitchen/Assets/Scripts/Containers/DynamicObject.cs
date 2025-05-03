using UnityEngine;

public class DynamicObject : ContainerObject<DynamicObject, DynamicContainer>
{
    public SmartAction<DynamicObject> OnSettled = new SmartAction<DynamicObject>();

    private bool _hasSettled = false;

    private void FixedUpdate()
    {
        // Invoke OnSettled every time the Rigidbody enters sleep mode
        if (!_hasSettled && Rigidbody.IsSleeping())
        {
            _hasSettled = true;
            OnSettled.Invoke(this);
        }
        else if (_hasSettled && !Rigidbody.IsSleeping())
        {
            _hasSettled = false;
        }
    }
}