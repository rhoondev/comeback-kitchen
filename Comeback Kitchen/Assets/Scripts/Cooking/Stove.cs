using Unity.VRTemplate;
using UnityEngine;

public enum StoveSetting
{
    Off,
    Low,
    MediumLow,
    Medium,
    MediumHigh,
    High
}

public class Stove : MonoBehaviour
{
    [SerializeField] private XRKnob xrKnob; // The interactable object that controls the knob
    [SerializeField] private Transform knobHandle; // The actual knob object
    [SerializeField] private InteractionLocker knobInteractionLocker; // The interaction locker for the knob
    [SerializeField] private Flame flame;

    public SmartAction<StoveSetting> OnSettingChanged = new SmartAction<StoveSetting>();

    private void Awake()
    {
        xrKnob.onValueChange.AddListener(OnKnobValueChanged);
    }

    public void LockKnob()
    {
        knobInteractionLocker.LockInteraction();
        Debug.Log("Knob locked");
    }

    public void UnlockKnob()
    {
        knobInteractionLocker.UnlockInteraction();
        Debug.Log("Knob unlocked");
    }

    private void OnKnobValueChanged(float _)
    {
        float knobRotation = Mod(knobHandle.transform.localEulerAngles.y, 360f); // Normalize the rotation to be between 0 and 360 degrees
        StoveSetting setting = GetStoveSetting(knobRotation);

        float flameSize = (knobRotation == 0f || knobRotation == 360f) ? 0f : 1f - knobRotation / 360f;
        flame.SetFlameSize(flameSize);

        Debug.Log($"Stove setting changed to: {setting}");

        OnSettingChanged.Invoke(setting);
    }

    private float Mod(float a, float b)
    {
        return ((a % b) + b) % b;
    }

    private StoveSetting GetStoveSetting(float rotation)
    {
        if (rotation >= 330f || rotation < 30f) return StoveSetting.Off; // 330 - 30
        if (rotation < 90f) return StoveSetting.High; // 30 - 90
        if (rotation < 150f) return StoveSetting.MediumHigh; // 90 - 150
        if (rotation < 210f) return StoveSetting.Medium; // 150 - 210
        if (rotation < 270f) return StoveSetting.MediumLow; // 210 - 270
        if (rotation < 330f) return StoveSetting.Low; // 270 - 330

        throw new System.ArgumentOutOfRangeException(nameof(rotation), rotation, null);
    }

    // private float GetKnobRotation(StoveSetting setting)
    // {
    //     switch (setting)
    //     {
    //         case StoveSetting.Off:
    //             return 0f;
    //         case StoveSetting.High:
    //             return 60f;
    //         case StoveSetting.MediumHigh:
    //             return 120f;
    //         case StoveSetting.Medium:
    //             return 180f;
    //         case StoveSetting.MediumLow:
    //             return 240f;
    //         case StoveSetting.Low:
    //             return 300f;
    //         default:
    //             throw new System.ArgumentOutOfRangeException(nameof(setting), setting, null);
    //     }
    // }
}
