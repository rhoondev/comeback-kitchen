using System.Collections;
using UnityEngine;

public class DynamicObject : ContainerObject<DynamicObject, DynamicContainer>
{
    public SmartAction<DynamicObject> OnSettle = new SmartAction<DynamicObject>();

    private bool _hasSettled = false;

    public override void OnRestore()
    {
        base.OnRestore();
        _hasSettled = false;
    }

    public override void OnTransfer()
    {
        _hasSettled = false;
    }

    protected override void OnWaitForRestore()
    {
        return; // Do nothing, let the object continue to move until it is restored
    }

    private void FixedUpdate()
    {
        if (!_hasSettled && Rigidbody.IsSleeping())
        {
            _hasSettled = true;
            OnSettle.Invoke(this);
        }
        else if (_hasSettled && !Rigidbody.IsSleeping())
        {
            _hasSettled = false;
        }
    }
}