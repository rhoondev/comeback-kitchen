using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Faucet : MonoBehaviour
{
    [SerializeField] private HingeJoint leverHingeJoint;
    [SerializeField] private InteractionLocker leverHingeInteractionLocker;
    [SerializeField] private XRGrabInteractable leverHingeInteractable;
    [SerializeField] private ParticleSystem stream;
    [SerializeField] private float maxFlowRate;

    public SmartAction OnTurnedFullyOn = new SmartAction();
    public SmartAction OnTurnedFullyOff = new SmartAction();

    private float _maxAngle;
    private float _flowRate;
    private bool _turnedFullyOn = false;
    private bool _turnedFullyOff = true;

    private void Start()
    {
        _maxAngle = leverHingeJoint.limits.max;
    }

    private void OnEnable()
    {
        leverHingeInteractable.selectEntered.AddListener(OnHingeSelectEntering);
        leverHingeInteractable.selectExited.AddListener(OnHingeSelectExiting);
    }

    private void OnDisable()
    {
        leverHingeInteractable.selectEntered.RemoveListener(OnHingeSelectEntering);
        leverHingeInteractable.selectExited.RemoveListener(OnHingeSelectExiting);
    }

    public void UnlockLever()
    {
        leverHingeInteractionLocker.UnlockInteraction();
    }

    public void LockLever()
    {
        leverHingeInteractionLocker.LockInteraction();
    }

    // Update is called once per frame
    private void Update()
    {
        float angle = leverHingeJoint.transform.localEulerAngles.z;
        float eps = 0.1f;

        if (angle >= _maxAngle - eps && !_turnedFullyOn)
        {
            _turnedFullyOn = true;
            _turnedFullyOff = false;
            OnTurnedFullyOn.Invoke();
        }
        else if (angle <= eps && !_turnedFullyOff)
        {
            _turnedFullyOff = true;
            _turnedFullyOn = false;
            OnTurnedFullyOff.Invoke();
        }
        else if (angle > eps && angle < _maxAngle - eps)
        {
            _turnedFullyOn = false;
            _turnedFullyOff = false;
        }

        _flowRate = angle / _maxAngle * maxFlowRate;

        var emission = stream.emission;
        emission.rateOverTimeMultiplier = _flowRate;
    }

    private void OnHingeSelectEntering(SelectEnterEventArgs args)
    {
        // Disable damper
        leverHingeJoint.useSpring = false;
    }

    private void OnHingeSelectExiting(SelectExitEventArgs args)
    {
        // Enable damper
        leverHingeJoint.useSpring = true;
    }
}
